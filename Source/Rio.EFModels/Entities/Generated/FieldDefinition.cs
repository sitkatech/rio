using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class FieldDefinition
    {
        [Column("FieldDefinitionID")]
        public int FieldDefinitionId { get; set; }
        [Required]
        [StringLength(300)]
        public string FieldDefinitionName { get; set; }
        [Required]
        [StringLength(300)]
        public string FieldDefinitionDisplayName { get; set; }
        [Required]
        public string DefaultDefinition { get; set; }
        public bool CanCustomizeLabel { get; set; }

        [InverseProperty("FieldDefinition")]
        public virtual FieldDefinitionData FieldDefinitionData { get; set; }
    }
}
