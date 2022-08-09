using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
        [Unicode(false)]
        public string TradeNumber { get; set; }

        [ForeignKey("CreateAccountID")]
        [InverseProperty("Trades")]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey("PostingID")]
        [InverseProperty("Trades")]
        public virtual Posting Posting { get; set; }
        [InverseProperty("Trade")]
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
