using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vParcelLayerUpdateDifferencesInAccountAssociatedWithParcel
    {
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [StringLength(255)]
        public string OldOwnerName { get; set; }
        [StringLength(100)]
        public string NewOwnerName { get; set; }
    }
}
