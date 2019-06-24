using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class FieldDefinitionDataImage
    {
        [Column("FieldDefinitionDataImageID")]
        public int FieldDefinitionDataImageId { get; set; }
        [Column("FieldDefinitionDataID")]
        public int FieldDefinitionDataId { get; set; }
        [Column("FileResourceID")]
        public int FileResourceId { get; set; }

        [ForeignKey("FieldDefinitionDataId")]
        [InverseProperty("FieldDefinitionDataImage")]
        public virtual FieldDefinitionData FieldDefinitionData { get; set; }
        [ForeignKey("FileResourceId")]
        [InverseProperty("FieldDefinitionDataImage")]
        public virtual FileResource FileResource { get; set; }
    }
}
