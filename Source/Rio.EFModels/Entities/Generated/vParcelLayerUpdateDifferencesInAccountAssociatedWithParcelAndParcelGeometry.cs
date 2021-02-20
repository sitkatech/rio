using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
    {
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        public int? WaterYearID { get; set; }
        [StringLength(255)]
        public string OldOwnerName { get; set; }
        [StringLength(100)]
        public string NewOwnerName { get; set; }
        public string OldGeometryText { get; set; }
        public string NewGeometryText { get; set; }
        public bool? HasConflict { get; set; }
    }
}
