using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistrationStatus
    {
        public WaterTransferRegistrationStatus()
        {
            WaterTransferRegistration = new HashSet<WaterTransferRegistration>();
        }

        [Key]
        public int WaterTransferRegistrationStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string WaterTransferRegistrationStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string WaterTransferRegistrationStatusDisplayName { get; set; }

        [InverseProperty("WaterTransferRegistrationStatus")]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistration { get; set; }
    }
}
