using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelDto
    {
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }

        public UserSimpleDto LandOwner { get; set; }
    }
}