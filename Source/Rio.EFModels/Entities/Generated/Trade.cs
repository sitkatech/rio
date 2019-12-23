using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class Trade
    {
        public Trade()
        {
            Offer = new HashSet<Offer>();
        }

        [Key]
        public int TradeID { get; set; }
        public int PostingID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TradeDate { get; set; }
        public int TradeStatusID { get; set; }
        [StringLength(50)]
        public string TradeNumber { get; set; }
        public int CreateAccountID { get; set; }

        [ForeignKey(nameof(CreateAccountID))]
        [InverseProperty(nameof(Account.Trade))]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey(nameof(PostingID))]
        [InverseProperty("Trade")]
        public virtual Posting Posting { get; set; }
        [ForeignKey(nameof(TradeStatusID))]
        [InverseProperty("Trade")]
        public virtual TradeStatus TradeStatus { get; set; }
        [InverseProperty("Trade")]
        public virtual ICollection<Offer> Offer { get; set; }
    }
}
