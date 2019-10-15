using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class RioPageType
    {
        [Key]
        public int RioPageTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string RioPageTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string RioPageTypeDisplayName { get; set; }
    }
}
