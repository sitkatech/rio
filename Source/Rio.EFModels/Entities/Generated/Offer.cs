using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class Offer
    {
        public int OfferID { get; set; }
        public int TradeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OfferDate { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public int OfferStatusID { get; set; }
        [StringLength(2000)]
        public string OfferNotes { get; set; }
        public int CreateUserID { get; set; }

        [ForeignKey("CreateUserID")]
        [InverseProperty("Offer")]
        public virtual User CreateUser { get; set; }
        [ForeignKey("OfferStatusID")]
        [InverseProperty("Offer")]
        public virtual OfferStatus OfferStatus { get; set; }
        [ForeignKey("TradeID")]
        [InverseProperty("Offer")]
        public virtual Trade Trade { get; set; }
    }
}
