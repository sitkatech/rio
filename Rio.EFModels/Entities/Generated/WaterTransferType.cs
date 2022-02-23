using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("WaterTransferType")]
    [Index(nameof(WaterTransferTypeDisplayName), Name = "AK_WaterTransferType_WaterTransferTypeDisplayName", IsUnique = true)]
    [Index(nameof(WaterTransferTypeName), Name = "AK_WaterTransferType_WaterTransferTypeName", IsUnique = true)]
    public partial class WaterTransferType
    {
        public WaterTransferType()
        {
            WaterTransferRegistrations = new HashSet<WaterTransferRegistration>();
        }

        [Key]
        public int WaterTransferTypeID { get; set; }
        [Required]
        [StringLength(50)]
        public string WaterTransferTypeName { get; set; }
        [Required]
        [StringLength(50)]
        public string WaterTransferTypeDisplayName { get; set; }

        [InverseProperty(nameof(WaterTransferRegistration.WaterTransferType))]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistrations { get; set; }
    }
}
