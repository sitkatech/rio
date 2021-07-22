using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("PostingStatus")]
    [Index(nameof(PostingStatusDisplayName), Name = "AK_PostingStatus_PostingStatusDisplayName", IsUnique = true)]
    [Index(nameof(PostingStatusName), Name = "AK_PostingStatus_PostingStatusName", IsUnique = true)]
    public partial class PostingStatus
    {
        public PostingStatus()
        {
            Postings = new HashSet<Posting>();
        }

        [Key]
        public int PostingStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingStatusDisplayName { get; set; }

        [InverseProperty(nameof(Posting.PostingStatus))]
        public virtual ICollection<Posting> Postings { get; set; }
    }
}
