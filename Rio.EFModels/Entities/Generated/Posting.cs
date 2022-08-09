using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("Posting")]
    public partial class Posting
    {
        public Posting()
        {
            Trades = new HashSet<Trade>();
        }

        [Key]
        public int PostingID { get; set; }
        public int PostingTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PostingDate { get; set; }
        public int CreateAccountID { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        [StringLength(2000)]
        [Unicode(false)]
        public string PostingDescription { get; set; }
        public int PostingStatusID { get; set; }
        public int AvailableQuantity { get; set; }
        public int? CreateUserID { get; set; }

        [ForeignKey("CreateAccountID")]
        [InverseProperty("Postings")]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey("CreateUserID")]
        [InverseProperty("Postings")]
        public virtual User CreateUser { get; set; }
        [InverseProperty("Posting")]
        public virtual ICollection<Trade> Trades { get; set; }
    }
}
