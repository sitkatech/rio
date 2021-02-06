using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.IO;
using Rio.Models.DataTransferObjects;
using static System.String;

namespace Rio.EFModels.Entities
{
    public partial class ParcelUpdateStaging
    {
        public static ParcelUpdateExpectedResultsDto AddFromFeatureCollection(RioDbContext _dbContext, FeatureCollection featureCollection, string validParcelNumberRegexPattern, string validParcelNumberAsStringForDisplay, WaterYearDto yearChangesToTakeEffect)
        {
            var commonColumnMappings = ParcelLayerGDBCommonMappingToParcelStagingColumn.GetCommonMappings(_dbContext);
            var wktWriter = new WKTWriter();
            
            //Create a datatable to use for bulk copy
            var dt = new DataTable();
            dt.Columns.Add("ParcelGeometryText", typeof(string));
            dt.Columns.Add("ParcelGeometry4326Text", typeof(string));
            dt.Columns.Add("OwnerName", typeof(string));
            dt.Columns.Add("ParcelNumber", typeof(string));
            dt.Columns.Add("HasConflict", typeof(bool));

            foreach (var feature in featureCollection)
            {
                var parcelNumber = feature.Attributes[commonColumnMappings.ParcelNumber].ToString();

                if (!Parcel.IsValidParcelNumber(validParcelNumberRegexPattern, parcelNumber))
                {
                    throw new ValidationException(
                        $"Parcel number found that does not comply to format {validParcelNumberAsStringForDisplay}. Please ensure that that correct column is selected and all Parcel Numbers follow the specified format and try again.");
                }

                //if it's an exact duplicate, there was some other info that wasn't relevant for our purposes and we can move along
                var geomAs4326Text = wktWriter.Write(feature.Geometry.ProjectTo4326());
                var ownerName = feature.Attributes[commonColumnMappings.OwnerName].ToString();
                if (dt.AsEnumerable().Any(x =>
                    x["ParcelGeometry4326Text"].ToString() == geomAs4326Text && x["OwnerName"].ToString() == ownerName &&
                    x["ParcelNumber"].ToString() == parcelNumber))
                {
                    continue;
                }

                dt.Rows.Add(
                    wktWriter.Write(feature.Geometry),
                    geomAs4326Text,
                    ownerName,
                    parcelNumber,
                    false
                );
            }

            var duplicates = dt.AsEnumerable().GroupBy(x => x[3]).Where(y => y.Count() > 1).ToList();

            if (duplicates.Any())
            {
                duplicates.ForEach(x =>
                {
                    var effectedRows = dt.Select("ParcelNumber='" + x.Key + "'");
                    var firstEffected = effectedRows.First();

                    if (effectedRows.Any(y => y["ParcelGeometry4326Text"].ToString() != firstEffected["ParcelGeometry4326Text"].ToString()))
                    {
                        throw new ValidationException(
                            $"Parcel number that has more than one geometry associated with it. Please ensure all Parcels have unique geometries and try uploading again.");
                    }

                    foreach (var row in effectedRows)
                    {
                        row["HasConflict"] = true;
                    }
                });
            }

            //Make sure staging table is empty before proceeding
            DeleteAll(_dbContext);

            var rioDbConnectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            using var destinationConnection = new SqlConnection(rioDbConnectionString);
            destinationConnection.Open();

            using var bulkCopy = new SqlBulkCopy(destinationConnection) { DestinationTableName = "dbo.ParcelUpdateStaging", EnableStreaming = true };
            //Specify columns so bulk copy knows exactly what it needs to do
            bulkCopy.ColumnMappings.Add("ParcelGeometryText", "ParcelGeometryText");
            bulkCopy.ColumnMappings.Add("ParcelGeometry4326Text", "ParcelGeometry4326Text");
            bulkCopy.ColumnMappings.Add("OwnerName", "OwnerName");
            bulkCopy.ColumnMappings.Add("ParcelNumber", "ParcelNumber");
            bulkCopy.ColumnMappings.Add("HasConflict", "HasConflict");

            bulkCopy.WriteToServer(dt);

            _dbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelUpdateStagingGeometryFromParcelGeometryText");

            return GetExpectedResultsDto(_dbContext, yearChangesToTakeEffect);
        }

        public static ParcelUpdateExpectedResultsDto GetExpectedResultsDto (RioDbContext _dbContext, WaterYearDto yearChangesToTakeEffect)
        {
            var accountsUpdatedView = _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Where(x =>
                !x.WaterYearID.HasValue || x.WaterYearID == yearChangesToTakeEffect.WaterYearID);

            var accountsUnchanged =
                accountsUpdatedView.Count(x =>
                    x.ExistingParcels.Equals(x.UpdatedParcels));
            var accountsToBeInactivated = accountsUpdatedView.Count(x =>
                x.AccountAlreadyExists.Value &&
                !IsNullOrEmpty(x.ExistingParcels) && IsNullOrEmpty(x.UpdatedParcels));
            var accountsToBeAdded = accountsUpdatedView.Count(x =>
                !x.AccountAlreadyExists.Value && IsNullOrEmpty(x.ExistingParcels) && !IsNullOrEmpty(x.UpdatedParcels));

            var parcelsUpdatedView = _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry.Where(x =>
                !x.WaterYearID.HasValue || x.WaterYearID == yearChangesToTakeEffect.WaterYearID);

            var parcelsUnchanged =
                parcelsUpdatedView
                    .Count(x => x.NewOwnerName.Equals(x.OldOwnerName) && x.NewGeometryText.Equals(x.OldGeometryText) && !x.HasConflict.Value);
            var parcelsWithChangedGeometries = parcelsUpdatedView
                .Count(x =>
                x.OldGeometryText != null && !x.OldGeometryText.Equals(x.NewGeometryText) && !x.HasConflict.Value);
            var parcelsAssociatedWithNewAccount = parcelsUpdatedView.Count(x =>
                !IsNullOrEmpty(x.NewOwnerName) && !x.NewOwnerName.Equals(x.OldOwnerName) && !x.HasConflict.Value);
            var parcelsToBeInactivated = parcelsUpdatedView.Count(x =>
                IsNullOrEmpty(x.NewOwnerName) && !x.HasConflict.Value);

            var numParcelsWithConflicts = _dbContext.ParcelUpdateStaging.Where(x => x.HasConflict).Select(x => x.ParcelNumber).Distinct().Count();

            var expectedChanges = new ParcelUpdateExpectedResultsDto()
            {
                NumAccountsUnchanged = accountsUnchanged,
                NumAccountsToBeCreated = accountsToBeAdded,
                NumAccountsToBeInactivated = accountsToBeInactivated,
                NumParcelsUnchanged = parcelsUnchanged,
                NumParcelsUpdatedGeometries = parcelsWithChangedGeometries,
                NumParcelsAssociatedWithNewAccount = parcelsAssociatedWithNewAccount,
                NumParcelsToBeInactivated = parcelsToBeInactivated,
                NumParcelsWithConflicts = numParcelsWithConflicts
            };

            return expectedChanges;
        }

        public static void DeleteAll(RioDbContext _dbContext)
        {
            _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE dbo.ParcelUpdateStaging");
            _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE dbo.AccountReconciliation");
        }
    }

    public class ParcelUpdateExpectedResultsDto
    {
        public int NumAccountsUnchanged { get; set; }
        public int NumAccountsToBeCreated { get; set; }
        public int NumAccountsToBeInactivated { get; set; }
        public int NumParcelsUnchanged { get; set; }
        public int NumParcelsUpdatedGeometries { get; set; }
        public int NumParcelsAssociatedWithNewAccount { get; set; }
        public int NumParcelsToBeInactivated { get; set; }
        public int NumParcelsWithConflicts { get; set; }
    }
}