using System;
using Newtonsoft.Json;

namespace Rio.Models.DataTransferObjects
{
    public class TradeActivityByMonthDto
    {
        public DateTime GroupingDate { get; set; }
        public decimal TradeVolume{ get; set; }
        public int NumberOfTrades{ get; set; }
    }
}