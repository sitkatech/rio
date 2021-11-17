using Rio.Models.DataTransferObjects.Account;
using System;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationSimpleDto
    {
        public int WaterTransferRegistrationID { get; set;}
        public int WaterTransferTypeID { get; set; }
        public AccountDto Account { get; set; }
        public int WaterTransferRegistrationStatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsPending { get; set; }
    }
}