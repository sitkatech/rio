using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferType
    {
        public WaterTransferType()
        {
            WaterTransferRegistration = new HashSet<WaterTransferRegistration>();
        }

        [Key]
        public int WaterTransferTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string WaterTransferTypeName { get; set; }
        [Required]
        [StringLength(50)]
        public string WaterTransferTypeDisplayName { get; set; }

        [InverseProperty("WaterTransferType")]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistration { get; set; }
    }
}
