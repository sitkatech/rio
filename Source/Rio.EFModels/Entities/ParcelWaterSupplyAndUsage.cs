using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.EFModels.Entities
{
    public partial class ParcelWaterSupplyAndUsage
    {
        public ParcelWaterSupplyAndUsage()
        {
        }

        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public decimal? TotalSupply { get; set; }
        public decimal? Purchased { get; set; }
        public decimal? Sold { get; set; }
        public decimal? UsageToDate { get; set; }
        public string AccountName { get; set; }
        public int? AccountID { get; set; }
        public int? AccountNumber { get; set; }

        public static IEnumerable<ParcelWaterSupplyAndUsageDto> GetByYear(RioDbContext dbContext, int year)
        {
            var sqlParameter = new SqlParameter("year", year);
            var parcelWaterSupplyAndUsages = dbContext.ParcelWaterSupplyAndUsages.FromSqlRaw($"EXECUTE dbo.pParcelWaterSupplyAndUsage @year", sqlParameter).ToList();

            var parcelWaterSupplyAndUsageDtos = parcelWaterSupplyAndUsages.OrderBy(x => x.ParcelNumber).Select(parcel =>
            {
                var parcelWaterSupplyAndUsageDto = new ParcelWaterSupplyAndUsageDto()
                {
                    ParcelID = parcel.ParcelID,
                    ParcelNumber = parcel.ParcelNumber,
                    ParcelAreaInAcres = parcel.ParcelAreaInAcres,
                    TotalSupply = parcel.TotalSupply,
                    Purchased = parcel.Purchased,
                    Sold = parcel.Sold,
                    UsageToDate = parcel.UsageToDate
                };

                if (parcel.AccountID.HasValue)
                {
                    parcelWaterSupplyAndUsageDto.LandOwner = new AccountDto()
                    {
                        AccountID = parcel.AccountID.Value,
                        AccountName = parcel.AccountName,
                        AccountNumber = parcel.AccountNumber.Value,
                        AccountDisplayName = $"#{parcel.AccountNumber} ({parcel.AccountName.Trim()})"
                    };
                }

                return parcelWaterSupplyAndUsageDto;
            });

            return parcelWaterSupplyAndUsageDtos;
        }
    }
}