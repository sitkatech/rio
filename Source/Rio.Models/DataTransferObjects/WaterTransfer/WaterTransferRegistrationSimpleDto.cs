using System;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationSimpleDto
    {
        public int WaterTransferTypeID { get; set; }
        public UserSimpleDto User { get; set; }
        public int WaterTransferRegistrationStatusID { get; set; }
        public DateTime StatusDate { get; set; }
        public bool IsRegistered { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsPending { get; set; }
    }
}