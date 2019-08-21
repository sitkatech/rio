using System;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferDto
    {
        public int WaterTransferID { get; set; }
        public int? OfferID { get; set; }
        public DateTime TransferDate { get; set; }
        public int TransferYear { get; set; }
        public decimal? AcreFeetTransferred { get; set; }
        public UserSimpleDto TransferringUser { get; set; }
        public UserSimpleDto ReceivingUser { get; set; }
        public bool ConfirmedByTransferringUser { get; set; }
        public bool ConfirmedByReceivingUser { get; set; }
        public string Notes { get; set; }
    }
}