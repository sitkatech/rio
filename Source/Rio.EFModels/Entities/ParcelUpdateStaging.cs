using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
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
        public static ParcelUpdateExpectedResultsDto AddFromFeatureCollection(RioDbContext _dbContext, FeatureCollection featureCollection, string validParcelNumberRegexPattern, string validParcelNumberAsStringForDisplay, int yearChangesToTakeEffect)
        {
            var commonColumnMappings = ParcelLayerGDBCommonMappingToParcelStagingColumn.GetCommonMappings(_dbContext);
            var wktWriter = new WKTWriter();
            
            //Create a datatable to use for bulk copy
            var dt = new DataTable();
            dt.Columns.Add("ParcelGeometryText", typeof(string));
            dt.Columns.Add("ParcelGeometry4326Text", typeof(string));
            dt.Columns.Add("OwnerName", typeof(string));
            dt.Columns.Add("ParcelNumber", typeof(string));

            foreach (var feature in featureCollection)
            {
                var parcelNumber = feature.Attributes[commonColumnMappings.ParcelNumber].ToString();

                if (!Parcel.IsValidParcelNumber(validParcelNumberRegexPattern, parcelNumber))
                {
                    throw new ValidationException(
                        $"Parcel number found that does not comply to format {validParcelNumberAsStringForDisplay}. Please ensure that that correct column is selected and all Parcel Numbers follow the specified format and try again.");
                }

                dt.Rows.Add(
                    wktWriter.Write(feature.Geometry),
                    wktWriter.Write(feature.Geometry.ProjectTo4326()),
                    feature.Attributes[commonColumnMappings.OwnerName].ToString(),
                    feature.Attributes[commonColumnMappings.ParcelNumber].ToString()
                );
            }

            if (dt.AsEnumerable().GroupBy(x => x[3]).Any(g => g.Count() > 1))
            {
                throw new ValidationException(
                    "There were duplicate Parcel Numbers found in the layer. Please ensure that all Parcel Numbers are unique and try uploading again.");
            }

            var inactiveParcelsFromParcelOwnership = _dbContext.vParcelOwnership.Include(x => x.Parcel).Where(x =>
                x.RowNumber == 1 && !x.AccountID.HasValue && IsNullOrEmpty(x.OwnerName) &&
                x.EffectiveYear.Value >= yearChangesToTakeEffect).Select(x => x.Parcel.ParcelNumber);
            if (dt.AsEnumerable().Any(x => inactiveParcelsFromParcelOwnership.Contains(x[3].ToString())))
            {
                throw new ValidationException(
                        "There were Parcel Numbers found that have been inactivated in a prior upload and cannot be associated with any new accounts. Please review the GDB and try again.");
            }

            var inactiveParcelsFromParcelOwnership = _dbContext.vParcelOwnership.Include(x => x.Parcel).Where(x =>
                x.RowNumber == 1 && !x.AccountID.HasValue && IsNullOrEmpty(x.OwnerName) &&
                x.EffectiveYear.Value >= yearChangesToTakeEffect).Select(x => x.Parcel.ParcelNumber);
            if (dt.AsEnumerable().Any(x => inactiveParcelsFromParcelOwnership.Contains(x[3].ToString())))
            {
                throw new ValidationException(
                        "There were Parcel Numbers found that have been inactivated in a prior upload and cannot be associated with any new accounts. Please review the GDB and try again.");
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

            bulkCopy.WriteToServer(dt);
            _dbContext.Database.ExecuteSqlRaw("EXECUTE dbo.pUpdateParcelUpdateStagingGeometryFromParcelGeometryText");

            return GetExpectedResultsDto(_dbContext);
        }

        public static ParcelUpdateExpectedResultsDto GetExpectedResultsDto (RioDbContext _dbContext)
        {
            var accountsUnchanged =
                _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Count(x =>
                    x.ExistingParcels.Equals(x.UpdatedParcels));
            var accountsToBeInactivated = _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Count(x =>
                x.AccountAlreadyExists.Value &&
                !IsNullOrEmpty(x.ExistingParcels) && IsNullOrEmpty(x.UpdatedParcels));
            var accountsToBeAdded = _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Count(x =>
                !x.AccountAlreadyExists.Value && IsNullOrEmpty(x.ExistingParcels) && !IsNullOrEmpty(x.UpdatedParcels));

            var parcelsUnchanged =
                _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
                    .Count(x => x.NewOwnerName.Equals(x.OldOwnerName) && x.NewGeometryText.Equals(x.OldGeometryText));
            var parcelsWithChangedGeometries = _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
                .Count(x =>
                x.OldGeometryText != null && !x.OldGeometryText.Equals(x.NewGeometryText));
            var parcelsAssociatedWithNewAccount = _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry.Count(x =>
                !IsNullOrEmpty(x.NewOwnerName) && !x.NewOwnerName.Equals(x.OldOwnerName));
            var parcelsToBeInactivated = _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry.Count(x =>
                IsNullOrEmpty(x.NewOwnerName));

            var expectedChanges = new ParcelUpdateExpectedResultsDto()
            {
                NumAccountsUnchanged = accountsUnchanged,
                NumAccountsToBeCreated = accountsToBeAdded,
                NumAccountsToBeInactivated = accountsToBeInactivated,
                NumParcelsUnchanged = parcelsUnchanged,
                NumParcelsUpdatedGeometries = parcelsWithChangedGeometries,
                NumParcelsAssociatedWithNewAccount = parcelsAssociatedWithNewAccount,
                NumParcelsToBeInactivated = parcelsToBeInactivated
            };

            return expectedChanges;
        }

        public static void DeleteAll(RioDbContext _dbContext)
        {
            _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE dbo.ParcelUpdateStaging");
        }
    }

    public class ParcelUpdateStagingComparer : IEqualityComparer<ParcelUpdateStaging>
    {

        public bool Equals(ParcelUpdateStaging x, ParcelUpdateStaging y)
        {
            return x.ParcelGeometryText.Equals(y.ParcelGeometryText) &&
                   x.ParcelNumber == y.ParcelNumber &&
                   x.OwnerName == y.OwnerName;
        }

        public int GetHashCode(ParcelUpdateStaging obj)
        {
            return obj.ParcelGeometryText.GetHashCode() ^
                   obj.ParcelNumber.GetHashCode() ^
                   obj.OwnerName.GetHashCode();
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
    }
}