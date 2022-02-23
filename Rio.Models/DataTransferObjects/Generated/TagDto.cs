//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Tag]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class TagDto
    {
        public int TagID { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
    }

    public partial class TagSimpleDto
    {
        public int TagID { get; set; }
        public string TagName { get; set; }
        public string TagDescription { get; set; }
    }

}