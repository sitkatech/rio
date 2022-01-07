using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;

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
        public decimal? TotalSupply { get; set; }
        public decimal? Precipitation { get; set; }
        public decimal? Purchased { get; set; }
        public decimal? Sold { get; set; }
        public decimal? UsageToDate { get; set; }
        public string AccountName { get; set; }
        public int? AccountID { get; set; }
        public int? AccountNumber { get; set; }

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
                    TotalSupply = parcel.TotalSupply,
                    Precipitation = parcel.Precipitation,
                    Purchased = parcel.Purchased,
                    Sold = parcel.Sold,
                    UsageToDate = parcel.UsageToDate
                };

                if (parcel.AccountID.HasValue)
                {
                    parcelAllocationAndUsageDto.LandOwner = new AccountDto()
                    {
                        AccountID = parcel.AccountID.Value,
                        AccountName = parcel.AccountName,
                        AccountNumber = parcel.AccountNumber.Value,
                        AccountDisplayName = $"#{parcel.AccountNumber} ({parcel.AccountName.Trim()})"
                    };
                }

                return parcelAllocationAndUsageDto;
            });

            return parcelAllocationAndUsageDtos;
        }
    }
}