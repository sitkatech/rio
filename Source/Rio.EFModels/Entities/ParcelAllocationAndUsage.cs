using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;

namespace Rio.EFModels.Entities
{
    public partial class ParcelAllocationAndUsage
    {
        public ParcelAllocationAndUsage()
        {
        }

        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public int? UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public decimal? ProjectWater { get; set; }
        public decimal? Reconciliation { get; set; }
        public decimal? NativeYield { get; set; }
        public decimal? StoredWater { get; set; }
        public decimal? Allocation { get; set; }
        public decimal? UsageToDate { get; set; }
        public string OwnerName { get; set; }

        public static IEnumerable<ParcelAllocationAndUsageDto> GetByYear(RioDbContext dbContext, int year)
        {
            var sqlParameter = new SqlParameter("year", year);
            var parcelAllocationAndUsages = dbContext.ParcelAllocationAndUsages.FromSqlRaw($"EXECUTE dbo.pParcelAllocationAndUsage @year", sqlParameter).ToList();

            var parcelAllocationAndUsageDtos = parcelAllocationAndUsages.OrderBy(x => x.ParcelNumber).Select(parcel =>
            {
                var parcelAllocationAndUsageDto = new ParcelAllocationAndUsageDto()
                {
                    ParcelID = parcel.ParcelID,
                    ParcelNumber = parcel.ParcelNumber,
                    ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                    ProjectWater = parcel.ProjectWater,
                    Reconciliation = parcel.Reconciliation,
                    NativeYield = parcel.NativeYield,
                    StoredWater = parcel.StoredWater,
                    Allocation = parcel.Allocation,
                    UsageToDate = parcel.UsageToDate,
                };
                // TODO
                //if (parcel.UserID.HasValue)
                //{
                //    parcelAllocationAndUsageDto.LandOwner = new UserSimpleDto()
                //    {
                //        FirstName = parcel.FirstName,
                //        LastName = parcel.LastName,
                //        Email = parcel.Email,
                //        UserID = parcel.UserID.Value
                //    };
                //}
                //else
                //{
                //    parcelAllocationAndUsageDto.LandOwner = new UserSimpleDto()
                //    {
                //        FirstName = parcel.OwnerName
                //    };
                //}

                return parcelAllocationAndUsageDto;
            });

            return parcelAllocationAndUsageDtos;
        }
    }
}