using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("WaterTransferRegistrationStatus")]
    [Index(nameof(WaterTransferRegistrationStatusDisplayName), Name = "AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusDisplayName", IsUnique = true)]
    [Index(nameof(WaterTransferRegistrationStatusName), Name = "AK_WaterTransferRegistrationStatus_WaterTransferRegistrationStatusName", IsUnique = true)]
    public partial class WaterTransferRegistrationStatus
    {
        public WaterTransferRegistrationStatus()
        {
            WaterTransferRegistrations = new HashSet<WaterTransferRegistration>();
        }

        [Key]
        public int WaterTransferRegistrationStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string WaterTransferRegistrationStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string WaterTransferRegistrationStatusDisplayName { get; set; }

        [InverseProperty(nameof(WaterTransferRegistration.WaterTransferRegistrationStatus))]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistrations { get; set; }
    }
}
