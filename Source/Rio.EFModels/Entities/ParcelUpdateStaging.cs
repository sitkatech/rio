using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;

namespace Rio.EFModels.Entities
{
    public partial class ParcelUpdateStaging
    {
        public static ParcelUpdateExpectedResultsDto AddFromFeatureCollection(RioDbContext _dbContext, FeatureCollection featureCollection)
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

            //We should throw errors if there are any duplicate ParcelNumbs, but eliminate that possibility if the Geometry, OwnerName and ParcelNumb are the same  
            parcelUpdateStagingEntities =
                parcelUpdateStagingEntities.Select(x => x).Distinct(new ParcelUpdateStagingComparer()).ToList();


            if (parcelUpdateStagingEntities.GroupBy(x => x.ParcelNumber).Any(g => g.Count() > 1))
            {
                throw new Exception(
                    "There were duplicate Parcel Numbers found in the layer. Please ensure that all Parcel Numbers are unique and try uploading again.");
            }

            var allParcels = _dbContext.Parcel.Select(x => new ParcelUpdateStaging()
            {
                ParcelNumber = x.ParcelNumber,
                OwnerName = x.OwnerName
            }).ToList();

            var currentParcelOwnership = _dbContext.vParcelOwnership
                .Include(x => x.Account)
                .Include(x => x.Parcel)
                .Where(x => x.RowNumber == 1)
                .Select(x => new ParcelUpdateStaging()
                {
                    ParcelNumber = x.Parcel.ParcelNumber,
                    OwnerName = x.Account.AccountName
                }).ToList();

            var currentParcelAccountAssociations = currentParcelOwnership.Union(
                allParcels.Where(x => currentParcelOwnership.All(y => y.ParcelNumber != x.ParcelNumber))).ToList();

            var expectedChanges = new ParcelUpdateExpectedResultsDto();
            var parcelNumbersAccountedFor = new List<string>();

            var distinctAccounts = parcelUpdateStagingEntities.Select(x => x.OwnerName)
                .Union(currentParcelAccountAssociations.Select(x => x.OwnerName)).Distinct();

            foreach (var account in distinctAccounts)
            {
                var currentResultsForAccount = currentParcelAccountAssociations.Where(x => x.OwnerName == account).ToList();
                var updatedResultsForAccount = parcelUpdateStagingEntities.Where(x => x.OwnerName == account).ToList();

                if (!updatedResultsForAccount.Any())
                {
                    expectedChanges.NumAccountsToBeInactivated++;
                    foreach (var parcel in currentResultsForAccount.Where(x => !parcelNumbersAccountedFor.Contains(x.ParcelNumber)).Select(x => x.ParcelNumber))
                    {
                        if (parcelUpdateStagingEntities.Any(x => x.ParcelNumber == parcel))
                        {
                            expectedChanges.NumParcelsAssociatedWithNewAccount++;
                        }
                        else
                        {
                            expectedChanges.NumParcelsToBeInactivated++;
                        }
                        parcelNumbersAccountedFor.Add(parcel);
                    }
                    continue;
                }

                if (!currentResultsForAccount.Any())
                {
                    expectedChanges.NumAccountsToBeCreated++;
                    var unaccountedForParcelNumbers = updatedResultsForAccount
                        .Where(x => !parcelNumbersAccountedFor.Contains(x.ParcelNumber) &&
                                    !currentParcelAccountAssociations.Exists(y => y.ParcelNumber == x.ParcelNumber))
                        .Select(x => x.OwnerName).ToList();
                    expectedChanges.NumParcelsAssociatedWithNewAccount += unaccountedForParcelNumbers.Count();
                    parcelNumbersAccountedFor.AddRange(unaccountedForParcelNumbers);
                    continue;
                }

                var distinctParcels = currentResultsForAccount.Select(x => x.ParcelNumber)
                    .Union(updatedResultsForAccount.Select(x => x.ParcelNumber)).Distinct().ToList();

                var accountChanged = false;

                foreach (var parcel in distinctParcels)
                {
                    if (currentResultsForAccount.Exists(x => x.ParcelNumber == parcel) && updatedResultsForAccount.Exists(x => x.ParcelNumber == parcel))
                    {
                        expectedChanges.NumParcelsUnchanged++;
                        parcelNumbersAccountedFor.Add(parcel);
                        continue;
                    }

                    accountChanged = true;

                    if (!parcelNumbersAccountedFor.Contains(parcel))
                    {
                        if (parcelUpdateStagingEntities.All(x => x.ParcelNumber != parcel))
                        {
                            expectedChanges.NumParcelsToBeInactivated++;
                        }
                        else
                        {
                            expectedChanges.NumParcelsAssociatedWithNewAccount++;
                        }
                        parcelNumbersAccountedFor.Add(parcel);
                    }
                }

                if (!accountChanged)
                {
                    expectedChanges.NumAccountsUnchanged++;
                }
            }

            _dbContext.ParcelUpdateStaging.AddRange(parcelUpdateStagingEntities);

            _dbContext.SaveChanges();

            return expectedChanges;
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
        public int NumParcelsAssociatedWithNewAccount { get; set; }
        public int NumParcelsToBeInactivated { get; set; }
    }
}