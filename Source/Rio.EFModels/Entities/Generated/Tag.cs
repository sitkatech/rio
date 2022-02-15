using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("Tag")]
    [Index(nameof(TagName), Name = "AK_Tag_TagName", IsUnique = true)]
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
        public string TagName { get; set; }
        [StringLength(500)]
        public string TagDescription { get; set; }

        [InverseProperty(nameof(ParcelTag.Tag))]
        public virtual ICollection<ParcelTag> ParcelTags { get; set; }
    }
}
