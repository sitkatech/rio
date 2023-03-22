//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelUsageFileUpload]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelUsageFileUploadDto
    {
        public int ParcelUsageFileUploadID { get; set; }
        public UserDto User { get; set; }
        public string UploadedFileName { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public int MatchedRecordCount { get; set; }
        public int UnmatchedParcelNumberCount { get; set; }
        public int NullParcelNumberCount { get; set; }
    }

    public partial class ParcelUsageFileUploadSimpleDto
    {
        public int ParcelUsageFileUploadID { get; set; }
        public int UserID { get; set; }
        public string UploadedFileName { get; set; }
        public DateTime UploadDate { get; set; }
        public DateTime? PublishDate { get; set; }
        public int MatchedRecordCount { get; set; }
        public int UnmatchedParcelNumberCount { get; set; }
        public int NullParcelNumberCount { get; set; }
    }

}