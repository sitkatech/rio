using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
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
