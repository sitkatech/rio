//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[CustomRichText]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class CustomRichTextDto
    {
        public int CustomRichTextID { get; set; }
        public CustomRichTextTypeDto CustomRichTextType { get; set; }
        public string CustomRichTextContent { get; set; }
    }

    public partial class CustomRichTextSimpleDto
    {
        public int CustomRichTextID { get; set; }
        public int CustomRichTextTypeID { get; set; }
        public string CustomRichTextContent { get; set; }
    }

}