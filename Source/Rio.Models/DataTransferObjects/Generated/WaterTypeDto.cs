//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[WaterType]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class WaterTypeDto
    {
        public int WaterTypeID { get; set; }
        public string WaterTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        public string WaterTypeDefinition { get; set; }
        public bool IsSourcedFromApi { get; set; }
        public int SortOrder { get; set; }
        public bool IsUserDefined { get; set; }
    }

    public partial class WaterTypeSimpleDto
    {
        public int WaterTypeID { get; set; }
        public string WaterTypeName { get; set; }
        public bool IsAppliedProportionally { get; set; }
        public string WaterTypeDefinition { get; set; }
        public bool IsSourcedFromApi { get; set; }
        public int SortOrder { get; set; }
        public bool IsUserDefined { get; set; }
    }

}