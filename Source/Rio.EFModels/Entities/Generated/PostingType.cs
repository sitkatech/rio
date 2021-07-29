using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("PostingType")]
    [Index(nameof(PostingTypeDisplayName), Name = "AK_PostingType_PostingTypeDisplayName", IsUnique = true)]
    [Index(nameof(PostingTypeName), Name = "AK_PostingType_PostingTypeName", IsUnique = true)]
    public partial class PostingType
    {
        public PostingType()
        {
            Postings = new HashSet<Posting>();
        }

        [Key]
        public int PostingTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingTypeDisplayName { get; set; }

        [InverseProperty(nameof(Posting.PostingType))]
        public virtual ICollection<Posting> Postings { get; set; }
    }
}
