using CsvHelper.Configuration;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.BulkSetAllocationCSV;

namespace Rio.API.Controllers
{
    public sealed class BulkSetAllocationCSVMap : ClassMap<BulkSetAllocationCSV>
    {
        public BulkSetAllocationCSVMap()
        {
            Map(m => m.APN).Name("APN");
            Map(m => m.AllocationQuantity).Name("Allocation Quantity");
        }

        public BulkSetAllocationCSVMap(RioDbContext dbContext, string waterTypeDisplayName)
        {
            Map(m => m.APN).Name("APN");
            Map(m => m.AllocationQuantity).Name(waterTypeDisplayName + " Quantity");
        }
    }
}