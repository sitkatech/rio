using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class CustomRichText
    {
        [Key]
        public int CustomRichTextID { get; set; }
        public int CustomRichTextTypeID { get; set; }
        public string CustomRichTextContent { get; set; }

        [ForeignKey(nameof(CustomRichTextTypeID))]
        [InverseProperty("CustomRichText")]
        public virtual CustomRichTextType CustomRichTextType { get; set; }
    }
}
