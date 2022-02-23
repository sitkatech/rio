//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Offer]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class OfferDto
    {
        public int OfferID { get; set; }
        public TradeDto Trade { get; set; }
        public DateTime OfferDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public OfferStatusDto OfferStatus { get; set; }
        public string OfferNotes { get; set; }
        public AccountDto CreateAccount { get; set; }
    }

    public partial class OfferSimpleDto
    {
        public int OfferID { get; set; }
        public int TradeID { get; set; }
        public DateTime OfferDate { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int OfferStatusID { get; set; }
        public string OfferNotes { get; set; }
        public int CreateAccountID { get; set; }
    }

}