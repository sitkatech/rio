namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelWithStatusDto : ParcelDto
    {
        public ParcelStatusDto ParcelStatus { get; set; }
    }
}