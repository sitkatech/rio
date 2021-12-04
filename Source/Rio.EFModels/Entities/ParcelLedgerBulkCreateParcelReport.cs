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
        public int AccountID { get; set; }
        public string AccountDisplayName { get; set; }
        public double Allocation { get; set; }
        public double ProjectWater { get; set; }
        public double NativeYield { get; set; }
        public double StoredWater { get; set; }
        public double Precipitation { get; set; }

        public static IEnumerable<ParcelLedgerBulkCreateParcelReportDto> GetAsDto(RioDbContext dbContext)
        {
            var ParcelLedgerBulkCreateParcelReports = dbContext.ParcelLedgerBulkCreateParcelReport.FromSqlRaw($"EXECUTE dbo.ParcelLedgerBulkCreateParcelReport").ToList();

            var ParcelLedgerBulkCreateParcelReportDtos = ParcelLedgerBulkCreateParcelReports.Select(x => new ParcelLedgerBulkCreateParcelReportDto
            {
                ParcelNumber = x.ParcelNumber,
                ParcelAreaInAcres = x.ParcelAreaInAcres,
                AccountID = x.AccountID,
                AccountDisplayName = x.AccountDisplayName,
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