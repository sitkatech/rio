using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class Posting
    {
        public Posting()
        {
            Trade = new HashSet<Trade>();
        }

        [Key]
        public int PostingID { get; set; }
        public int PostingTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PostingDate { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        [StringLength(2000)]
        public string PostingDescription { get; set; }
        public int PostingStatusID { get; set; }
        public int AvailableQuantity { get; set; }
        public int CreateAccountID { get; set; }

        [ForeignKey(nameof(CreateAccountID))]
        [InverseProperty(nameof(Account.Posting))]
        public virtual Account CreateAccount { get; set; }
        [ForeignKey(nameof(PostingStatusID))]
        [InverseProperty("Posting")]
        public virtual PostingStatus PostingStatus { get; set; }
        [ForeignKey(nameof(PostingTypeID))]
        [InverseProperty("Posting")]
        public virtual PostingType PostingType { get; set; }
        [InverseProperty("Posting")]
        public virtual ICollection<Trade> Trade { get; set; }
    }
}
