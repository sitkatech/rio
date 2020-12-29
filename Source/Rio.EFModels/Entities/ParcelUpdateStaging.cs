using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Operation.Valid;
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
                var isValidOp = new IsValidOp(feature.Geometry);
                if (!isValidOp.IsValid)
                {
                    var blah = feature.Geometry.AsText();
                    Debug.WriteLine("feature.Geometry is not valid:" + isValidOp.ValidationError.Message);
                    feature.Geometry = feature.Geometry.Buffer(0);
                }
                var newParcelStagingEntity = new ParcelUpdateStaging()
                {
                    ParcelGeometry =  feature.Geometry,
                    OwnerName = feature.Attributes[commonColumnMappings.OwnerName].ToString(),
                    ParcelNumber = feature.Attributes[commonColumnMappings.ParcelNumber].ToString(),
                };
                parcelUpdateStagingEntities.Add(newParcelStagingEntity);
            }

            //We should throw errors if there are any duplicate ParcelNumbs, but eliminate that possibility if the Geometry, OwnerName and ParcelNumb are the same  
            parcelUpdateStagingEntities =
                parcelUpdateStagingEntities.Select(x => x).Distinct(new ParcelUpdateStagingComparer()).ToList();

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
            return x.ParcelGeometry.EqualsExact(y.ParcelGeometry) &&
                   x.ParcelNumber == y.ParcelNumber &&
                   x.OwnerName == y.OwnerName;
        }

        public int GetHashCode(ParcelUpdateStaging obj)
        {
            return obj.ParcelGeometry.GetHashCode() ^
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