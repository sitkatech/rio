//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[ParcelTag]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelTagDto
    {
        public int ParcelTagID { get; set; }
        public ParcelDto Parcel { get; set; }
        public TagDto Tag { get; set; }
    }

    public partial class ParcelTagSimpleDto
    {
        public int ParcelTagID { get; set; }
        public int ParcelID { get; set; }
        public int TagID { get; set; }
    }

}