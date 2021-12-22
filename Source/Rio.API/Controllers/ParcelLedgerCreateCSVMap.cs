using CsvHelper.Configuration;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;

namespace Rio.API.Controllers
{
    public sealed class ParcelLedgerCreateCSVMap : ClassMap<ParcelLedgerCreateCSV>
    {
        public ParcelLedgerCreateCSVMap(RioDbContext dbContext, string waterTypeDisplayName)
        {
            Map(m => m.APN).Name("APN");
            Map(m => m.Quantity).Name(waterTypeDisplayName + " Quantity");
        }
    }
}