using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class FieldDefinitionData
    {
        public FieldDefinitionData()
        {
            FieldDefinitionDataImage = new HashSet<FieldDefinitionDataImage>();
        }

        [Column("FieldDefinitionDataID")]
        public int FieldDefinitionDataId { get; set; }
        [Column("FieldDefinitionID")]
        public int FieldDefinitionId { get; set; }
        public string FieldDefinitionDataValue { get; set; }
        [StringLength(300)]
        public string FieldDefinitionLabel { get; set; }

        [ForeignKey("FieldDefinitionId")]
        [InverseProperty("FieldDefinitionData")]
        public virtual FieldDefinition FieldDefinition { get; set; }
        [InverseProperty("FieldDefinitionData")]
        public virtual ICollection<FieldDefinitionDataImage> FieldDefinitionDataImage { get; set; }
    }
}
