using System;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class OfferDto
    {
        public int OfferID { get; set; }
        public DateTime OfferDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string OfferNotes { get; set; }
        public UserSimpleDto CreateUser { get; set; }
        public OfferStatusDto OfferStatus { get; set; }
        public int TradeID { get; set; }
        public bool RegisteredBySeller { get; set; }
        public DateTime? DateRegisteredBySeller { get; set; }
        public bool RegisteredByBuyer { get; set; }
        public DateTime? DateRegisteredByBuyer { get; set; }
        public int? WaterTransferID { get; set; }
    }
}
