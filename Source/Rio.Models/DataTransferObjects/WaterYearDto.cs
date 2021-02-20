using System;
using System.Collections.Generic;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.Models.DataTransferObjects
{
    public class WaterYearDto
    {
        public int WaterYearID { get; set; }
        public int Year { get; set; }
        public DateTime? FinalizeDate { get; set; }
        public DateTime? ParcelLayerUpdateDate { get; set; }
    }

    public class AccountReconciliationDto
    {
        public ParcelSimpleDto Parcel { get; set; }
        public AccountSimpleDto LastKnownOwner { get; set; }
        public List<AccountSimpleDto> AccountsClaimingOwnership { get; set; }
    }
}