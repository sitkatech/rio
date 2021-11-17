using System;

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
        public string Notes { get; set; }
        public string TradeNumber { get; set; }

        public WaterTransferRegistrationSimpleDto BuyerRegistration { get; set; }
        public WaterTransferRegistrationSimpleDto SellerRegistration { get; set; }
    }
}