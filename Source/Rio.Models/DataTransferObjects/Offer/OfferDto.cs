using Rio.Models.DataTransferObjects.Account;
using System;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class OfferDto
    {
        public int OfferID { get; set; }
        public DateTime OfferDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string OfferNotes { get; set; }
        public AccountDto CreateAccount { get; set; }
        public OfferStatusDto OfferStatus { get; set; }
        public int TradeID { get; set; }
        public int? WaterTransferID { get; set; }
    }
}
