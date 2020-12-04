using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class WaterYearQuickOpenETHistoryDto
    {
        public WaterYearDto WaterYear { get; set; }
        public bool CurrentlySyncing { get; set; }
        public DateTime? LastSuccessfulSync { get; set; }
    }
}
