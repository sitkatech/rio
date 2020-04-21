using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class CustomRichTextType
    {
        public CustomRichTextType()
        {
            CustomRichText = new HashSet<CustomRichText>();
        }

        [Key]
        public int CustomRichTextTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string CustomRichTextTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string CustomRichTextTypeDisplayName { get; set; }

        [InverseProperty("CustomRichTextType")]
        public virtual ICollection<CustomRichText> CustomRichText { get; set; }
    }
}
