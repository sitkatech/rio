using System;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferDto
    {
        public int WaterTransferID { get; set; }
        public int OfferID { get; set; }
        public DateTime TransferDate { get; set; }
        public int TransferYear { get; set; }
        public int AcreFeetTransferred { get; set; }
        public decimal? UnitPrice { get; set; }
        public UserSimpleDto Seller { get; set; }
        public UserSimpleDto Buyer { get; set; }
        public bool RegisteredBySeller { get; set; }
        public DateTime? DateRegisteredBySeller { get; set; }
        public bool RegisteredByBuyer { get; set; }
        public DateTime? DateRegisteredByBuyer { get; set; }
        public string Notes { get; set; }
        public string TradeNumber { get; set; }
    }
}