//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Parcel]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class ParcelDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public int ParcelAreaInSquareFeet { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public ParcelStatusDto ParcelStatus { get; set; }
        public DateTime? InactivateDate { get; set; }
    }

    public partial class ParcelSimpleDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public int ParcelAreaInSquareFeet { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public int ParcelStatusID { get; set; }
        public DateTime? InactivateDate { get; set; }
    }

}