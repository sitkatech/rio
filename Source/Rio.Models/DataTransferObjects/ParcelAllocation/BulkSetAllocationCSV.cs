using System;
using System.Text;

namespace Rio.Models.DataTransferObjects.BulkSetAllocationCSV
{
    //Keeping as reference for if we allow by account
    //public class BulkSetAllocationCSV
    //{
    //    public int AccountNumber { get; set; }
    //    public double? AllocationVolume { get; set; }
    //}

    public class BulkSetAllocationCSV
    {
        public string APN { get; set; }
        public double? AllocationQuantity { get; set; }
    }
}