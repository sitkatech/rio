using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("Trade")]
    public partial class Trade
    {
        public Trade()
        {
            Offers = new HashSet<Offer>();
        }

        [Key]
        public int TradeID { get; set; }
        public int PostingID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TradeDate { get; set; }
        public int TradeStatusID { get; set; }
        public int CreateAccountID { get; set; }
        [StringLength(50)]
        public string TradeNumber { get; set; }

        [ForeignKey(nameof(CreateAccountID))]
        [InverseProperty(nameof(Account.Trades))]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey(nameof(PostingID))]
        [InverseProperty("Trades")]
        public virtual Posting Posting { get; set; }
        [ForeignKey(nameof(TradeStatusID))]
        [InverseProperty("Trades")]
        public virtual TradeStatus TradeStatus { get; set; }
        [InverseProperty(nameof(Offer.Trade))]
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
