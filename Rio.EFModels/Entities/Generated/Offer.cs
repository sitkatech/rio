using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("Offer")]
    public partial class Offer
    {
        public Offer()
        {
            WaterTransfers = new HashSet<WaterTransfer>();
        }

        [Key]
        public int OfferID { get; set; }
        public int TradeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime OfferDate { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public int OfferStatusID { get; set; }
        [StringLength(2000)]
        [Unicode(false)]
        public string OfferNotes { get; set; }
        public int CreateAccountID { get; set; }

        [ForeignKey("CreateAccountID")]
        [InverseProperty("Offers")]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey("TradeID")]
        [InverseProperty("Offers")]
        public virtual Trade Trade { get; set; }
        [InverseProperty("Offer")]
        public virtual ICollection<WaterTransfer> WaterTransfers { get; set; }
    }
}
