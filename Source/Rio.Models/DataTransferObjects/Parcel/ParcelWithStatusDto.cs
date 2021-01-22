using System;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelWithStatusDto : ParcelDto
    {
        public int ParcelStatusID { get; set; }
        public DateTime? InactivateDate { get; set; }
    }
}