//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UploadedGdb]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class UploadedGdbDto
    {
        public int UploadedGdbID { get; set; }
        public byte[] GdbFileContents { get; set; }
        public DateTime UploadDate { get; set; }
    }

    public partial class UploadedGdbSimpleDto
    {
        public int UploadedGdbID { get; set; }
        public byte[] GdbFileContents { get; set; }
        public DateTime UploadDate { get; set; }
    }

}