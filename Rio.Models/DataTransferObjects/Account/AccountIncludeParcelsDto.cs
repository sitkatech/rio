using System.Collections.Generic;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.Models.DataTransferObjects.Account
{
    public class AccountIncludeParcelsDto
    {
        public AccountDto Account { get; set; }
        public List<ParcelSimpleDto> Parcels { get; set; }
    }
}
