//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Well]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WellDto
    {
        public int WellID { get; set; }
        public string WellName { get; set; }
        public string WellType { get; set; }
        public int WellTypeCode { get; set; }
        public string WellTypeCodeName { get; set; }
    }

    public partial class WellSimpleDto
    {
        public int WellID { get; set; }
        public string WellName { get; set; }
        public string WellType { get; set; }
        public int WellTypeCode { get; set; }
        public string WellTypeCodeName { get; set; }
    }

}