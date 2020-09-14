using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class ParcelAllocationTypeDto
    {
        public int ParcelAllocationTypeID { get; set; }
        public string ParcelAllocationTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
    }
}
