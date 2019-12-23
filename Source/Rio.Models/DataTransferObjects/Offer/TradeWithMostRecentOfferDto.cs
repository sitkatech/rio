using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class TradeWithMostRecentOfferDto
    {
        public int TradeID { get; set; }
        public string TradeNumber { get; set; }
        public AccountSimpleDto CreateUser { get; set; }
        public int OfferPostingTypeID { get; set; }
        public TradeStatusDto TradeStatus { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OfferDate { get; set; }
        public AccountSimpleDto OfferCreateAccount { get; set; }
        public OfferStatusDto OfferStatus { get; set; }
        public AccountSimpleDto Buyer { get; set; }
        public AccountSimpleDto Seller { get; set; }
        public int TradePostingTypeID { get; set; }
        public int? WaterTransferID { get; set; }
        public WaterTransferRegistrationSimpleDto BuyerRegistration { get; set; }
        public WaterTransferRegistrationSimpleDto SellerRegistration { get; set; }
    }
}