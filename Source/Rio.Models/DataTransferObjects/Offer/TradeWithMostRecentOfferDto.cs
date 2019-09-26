using System;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class TradeWithMostRecentOfferDto
    {
        public int TradeID { get; set; }
        public string TradeNumber { get; set; }
        public UserSimpleDto CreateUser { get; set; }
        public int OfferPostingTypeID { get; set; }
        public TradeStatusDto TradeStatus { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OfferDate { get; set; }
        public UserSimpleDto OfferCreateUser { get; set; }
        public OfferStatusDto OfferStatus { get; set; }
        public UserSimpleDto Buyer { get; set; }
        public UserSimpleDto Seller { get; set; }
        public int TradePostingTypeID { get; set; }
        public bool IsRegisteredByBuyer { get; set; }
        public bool IsRegisteredBySeller { get; set; }
        public int? WaterTransferID { get; set; }
    }
}