using System;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class TradeWithMostRecentOfferDto
    {
        public int TradeID { get; set; }
        public UserSimpleDto CreateUser { get; set; }
        public TradeStatusDto TradeStatus { get; set; }

        public int OfferID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime OfferDate{ get; set; }
        public int OfferCreateUserID{ get; set; }
        public OfferStatusDto OfferStatus { get; set; }
        public PostingDto Posting { get; set; }
    }
}