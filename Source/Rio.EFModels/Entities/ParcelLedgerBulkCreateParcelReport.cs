using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class ParcelLedgerBulkCreateParcelReport
    {
        public ParcelLedgerBulkCreateParcelReport()
        {
        }

        public string ParcelNumber { get; set; } 
        public decimal ParcelAreaInAcres { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public double Allocation { get; set; }
        public double ProjectWater { get; set; }
        public double NativeYield { get; set; }
        public double StoredWater { get; set; }
        public double Precipitation { get; set; }

        public static IEnumerable<ParcelLedgerBulkCreateParcelReportDto> GetAsDto(RioDbContext dbContext)
        {
            var ParcelLedgerBulkCreateParcelReports = dbContext.ParcelLedgerBulkCreateParcelReport.FromSqlRaw($"EXECUTE dbo.ParcelLedgerBulkCreateParcelReport").ToList();

            var ParcelLedgerBulkCreateParcelReportDtos = ParcelLedgerBulkCreateParcelReports.OrderBy(x => x.AccountNumber).Select(x => new ParcelLedgerBulkCreateParcelReportDto
            {
                ParcelNumber = x.ParcelNumber,
                ParcelAreaInAcres = x.ParcelAreaInAcres,
                AccountNumber = x.AccountNumber,
                AccountName = x.AccountName,
                Allocation = x.Allocation,
                ProjectWater = x.ProjectWater,
                NativeYield = x.NativeYield,
                StoredWater = x.StoredWater,
                Precipitation = x.Precipitation
            });

            return ParcelLedgerBulkCreateParcelReportDtos;
        }
    }
}