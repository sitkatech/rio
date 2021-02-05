using Rio.Models.DataTransferObjects.Account;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelOwnershipDto
    {
        public AccountSimpleDto Account { get; set; }
        public WaterYearDto WaterYear { get; set; }
    }
}
