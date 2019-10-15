using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class PostingType
    {
        public PostingType()
        {
            Posting = new HashSet<Posting>();
        }

        [Key]
        public int PostingTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingTypeDisplayName { get; set; }

        [InverseProperty("PostingType")]
        public virtual ICollection<Posting> Posting { get; set; }
    }
}
