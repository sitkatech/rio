using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using static System.String;

namespace Rio.EFModels.Entities
{
    public partial class ParcelUpdateStaging
    {
        public static ParcelUpdateExpectedResultsDto AddFromFeatureCollection(RioDbContext _dbContext, FeatureCollection featureCollection, string validParcelNumberRegexPattern, string validParcelNumberAsStringForDisplay)
        {
            var commonColumnMappings = ParcelLayerGDBCommonMappingToParcelStagingColumn.GetCommonMappings(_dbContext);

            var parcelUpdateStagingEntities = new List<ParcelUpdateStaging>();
            foreach (var feature in featureCollection)
            {
                var newParcelStagingEntity = new ParcelUpdateStaging()
                {
                    ParcelGeometry = feature.Geometry,
                    OwnerName = feature.Attributes[commonColumnMappings.OwnerName].ToString(),
                    ParcelNumber = feature.Attributes[commonColumnMappings.ParcelNumber].ToString(),
                };
                parcelUpdateStagingEntities.Add(newParcelStagingEntity);
            }

            if (parcelUpdateStagingEntities.GroupBy(x => x.ParcelNumber).Any(g => g.Count() > 1))
            {
                throw new ValidationException(
                    "There were duplicate Parcel Numbers found in the layer. Please ensure that all Parcel Numbers are unique and try uploading again.");
            }

            if (parcelUpdateStagingEntities.Any(x => !Parcel.IsValidParcelNumber(validParcelNumberRegexPattern, x.ParcelNumber)))
            {
                throw new ValidationException(
                    $"Parcel number found that does not comply to format {validParcelNumberAsStringForDisplay}. Please ensure that that correct column is selected and all Parcel Numbers follow the specified format and try again.");
            }

            //Make sure staging table is empty before proceeding
            DeleteAll(_dbContext);

            _dbContext.ParcelUpdateStaging.AddRange(parcelUpdateStagingEntities);

            _dbContext.SaveChanges();

            return GetExpectedResultsDto(_dbContext);
        }

        public static ParcelUpdateExpectedResultsDto GetExpectedResultsDto (RioDbContext _dbContext)
        {
            var accountsUnchanged =
                _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Count(x =>
                    x.ExistingParcels.Equals(x.UpdatedParcels));
            var accountsToBeInactivated = _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Count(x =>
                !IsNullOrEmpty(x.ExistingParcels) && IsNullOrEmpty(x.UpdatedParcels));
            var accountsToBeAdded = _dbContext.vParcelLayerUpdateDifferencesInParcelsAssociatedWithAccount.Count(x =>
                IsNullOrEmpty(x.ExistingParcels) && !IsNullOrEmpty(x.UpdatedParcels));

            var parcelsUnchanged =
                _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel.Count(x =>
                    x.NewOwnerName.Equals(x.OldOwnerName));
            var parcelsAssociatedWithNewAccount = _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel.Count(x =>
                !IsNullOrEmpty(x.NewOwnerName) && !x.NewOwnerName.Equals(x.OldOwnerName));
            var parcelsToBeInactivated = _dbContext.vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel.Count(x =>
                IsNullOrEmpty(x.NewOwnerName));

            var expectedChanges = new ParcelUpdateExpectedResultsDto()
            {
                NumAccountsUnchanged = accountsUnchanged,
                NumAccountsToBeCreated = accountsToBeAdded,
                NumAccountsToBeInactivated = accountsToBeInactivated,
                NumParcelsUnchanged = parcelsUnchanged,
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

    public class ParcelUpdateExpectedResultsDto
    {
        public int NumAccountsUnchanged { get; set; }
        public int NumAccountsToBeCreated { get; set; }
        public int NumAccountsToBeInactivated { get; set; }
        public int NumParcelsUnchanged { get; set; }
        public int NumParcelsAssociatedWithNewAccount { get; set; }
        public int NumParcelsToBeInactivated { get; set; }
    }
}