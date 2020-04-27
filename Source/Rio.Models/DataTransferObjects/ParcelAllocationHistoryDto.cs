using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class ParcelAllocationHistoryDto
    {
        public DateTime Date;
        public int WaterYear;
        public string Allocation;
        public decimal? Value;
        public string? Filename;
        public string User;
    }
}
