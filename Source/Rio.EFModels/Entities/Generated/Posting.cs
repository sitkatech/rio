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

        public int PostingID { get; set; }
        public int PostingTypeID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PostingDate { get; set; }
        public int CreateUserID { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        [Required]
        [StringLength(2000)]
        public string PostingDescription { get; set; }

        [ForeignKey("CreateUserID")]
        [InverseProperty("Posting")]
        public virtual User CreateUser { get; set; }
        [ForeignKey("PostingTypeID")]
        [InverseProperty("Posting")]
        public virtual PostingType PostingType { get; set; }
        [InverseProperty("Posting")]
        public virtual ICollection<Trade> Trade { get; set; }
    }
}
