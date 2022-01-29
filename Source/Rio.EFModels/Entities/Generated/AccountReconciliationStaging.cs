using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AccountReconciliationStaging
    {
        [Key]
        public int AccountReconciliationStagingID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [Required]
        [StringLength(255)]
        public string OwnerName { get; set; }
    }
}
