using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class PostingStatus
    {
        public PostingStatus()
        {
            Posting = new HashSet<Posting>();
        }

        [Key]
        public int PostingStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingStatusDisplayName { get; set; }

        [InverseProperty("PostingStatus")]
        public virtual ICollection<Posting> Posting { get; set; }
    }
}
