using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

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
        public string PostingDescription { get; set; }
        public int PostingStatusID { get; set; }
        public int AvailableQuantity { get; set; }
        public int? CreateUserID { get; set; }

        [ForeignKey(nameof(CreateAccountID))]
        [InverseProperty(nameof(Account.Postings))]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey(nameof(CreateUserID))]
        [InverseProperty(nameof(User.Postings))]
        public virtual User CreateUser { get; set; }
        [ForeignKey(nameof(PostingStatusID))]
        [InverseProperty("Postings")]
        public virtual PostingStatus PostingStatus { get; set; }
        [ForeignKey(nameof(PostingTypeID))]
        [InverseProperty("Postings")]
        public virtual PostingType PostingType { get; set; }
        [InverseProperty(nameof(Trade.Posting))]
        public virtual ICollection<Trade> Trades { get; set; }
    }
}
