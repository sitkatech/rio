using System;
using System.Collections.Generic;
using System.Text;

namespace Rio.Models.DataTransferObjects
{
    public class ParcelAllocationTypeDto
    {
        public int ParcelAllocationTypeID { get; set; }
        public string ParcelAllocationTypeName { get; set; }
        public ParcelAllocationTypeApplicationTypeEnum IsAppliedProportionally { get; set; }
        public string ParcelAllocationTypeDefinition { get; set; }
        public int SortOrder { get; set; }
    }

    public enum ParcelAllocationTypeApplicationTypeEnum
    {
        Spreadsheet = 0,
        Proportionally = 1,
        Api = 2
    }
}
