using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Keyless]
    public partial class vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
    {
        [StringLength(20)]
        [Unicode(false)]
        public string ParcelNumber { get; set; }
        public int? WaterYearID { get; set; }
        [StringLength(255)]
        [Unicode(false)]
        public string OldOwnerName { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string NewOwnerName { get; set; }
        public string OldGeometryText { get; set; }
        public string NewGeometryText { get; set; }
        public bool? HasConflict { get; set; }
    }
}
