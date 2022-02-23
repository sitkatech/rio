using System.Collections.Generic;

namespace Rio.Models.DataTransferObjects
{
    public class AccountReconciliationCustomDto
    {
        public ParcelSimpleDto Parcel { get; set; }
        public AccountSimpleDto LastKnownOwner { get; set; }
        public List<AccountSimpleDto> AccountsClaimingOwnership { get; set; }
    }
}