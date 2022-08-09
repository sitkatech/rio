using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("Tag")]
    [Index("TagName", Name = "AK_Tag_TagName", IsUnique = true)]
    public partial class Tag
    {
        public Tag()
        {
            ParcelTags = new HashSet<ParcelTag>();
        }

        [Key]
        public int TagID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string TagName { get; set; }
        [StringLength(500)]
        [Unicode(false)]
        public string TagDescription { get; set; }

        [InverseProperty("Tag")]
        public virtual ICollection<ParcelTag> ParcelTags { get; set; }
    }
}
