using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public partial class LandownerUsageReport
    {
        public LandownerUsageReport()
        {
        }

        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public decimal? Allocation { get; set; }
        public int? Purchased { get; set; }
        public int? Sold { get; set; }
        public decimal? TotalSupply { get; set; }
        public decimal? UsageToDate { get; set; }
        public decimal? CurrentAvailable { get; set; }
        public int NumberOfPostings { get; set; }
        public int NumberOfTrades { get; set; }
        public string MostRecentTradeNumber { get; set; }

        public static IEnumerable<LandownerUsageReportDto> GetByYear(RioDbContext dbContext, int year)
        {
            var sqlParameter = new SqlParameter("year", year);
            var landownerUsageReports = dbContext.LandownerUsageReports.FromSqlRaw($"EXECUTE dbo.pLandownerUsageReport @year", sqlParameter).ToList();

            var landownerUsageReportDtos = landownerUsageReports.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Select(x => new LandownerUsageReportDto()
            {
                UserID = x.UserID,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Allocation = x.Allocation,
                Purchased = x.Purchased,
                Sold = x.Sold,
                TotalSupply = x.TotalSupply,
                UsageToDate = x.UsageToDate,
                CurrentAvailable = x.CurrentAvailable,
                NumberOfPostings = x.NumberOfPostings,
                NumberOfTrades = x.NumberOfTrades,
                MostRecentTradeNumber = x.MostRecentTradeNumber
            });

            return landownerUsageReportDtos;
        }
    }
}