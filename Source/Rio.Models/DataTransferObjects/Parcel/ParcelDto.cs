using Rio.Models.DataTransferObjects.Account;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelDto
    {
        public int ParcelID { get; set; }
        
        public string ParcelNumber { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public AccountSimpleDto LandOwner { get; set; }
    }
}
