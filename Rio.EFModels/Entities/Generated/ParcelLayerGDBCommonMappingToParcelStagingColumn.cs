using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelLayerGDBCommonMappingToParcelStagingColumn")]
    [Index(nameof(OwnerName), Name = "AK_ParcelLayerGDBCommonMappingToParcelColumn_OwnerName", IsUnique = true)]
    [Index(nameof(ParcelNumber), Name = "AK_ParcelLayerGDBCommonMappingToParcelColumn_ParcelNumber", IsUnique = true)]
    public partial class ParcelLayerGDBCommonMappingToParcelStagingColumn
    {
        [Key]
        public int ParcelLayerGDBCommonMappingToParcelColumnID { get; set; }
        [Required]
        [StringLength(100)]
        public string ParcelNumber { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; }
    }
}
