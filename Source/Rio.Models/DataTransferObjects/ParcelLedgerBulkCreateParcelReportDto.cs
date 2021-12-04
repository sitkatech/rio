﻿namespace Rio.Models.DataTransferObjects
{
    public class ParcelLedgerBulkCreateParcelReportDto
    {
        public string ParcelNumber { get; set; }
        public decimal ParcelAreaInAcres { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public double Allocation { get; set; }
        public double ProjectWater { get; set; }
        public double NativeYield { get; set; }
        public double StoredWater { get; set; }
        public double Precipitation { get; set; }
    }
}