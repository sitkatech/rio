﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class vParcelLayerUpdateDifferencesInAccountAssociatedWithParcelAndParcelGeometry
    {
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [StringLength(255)]
        public string OldOwnerName { get; set; }
        [StringLength(100)]
        public string NewOwnerName { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry OldGeometry { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry NewGeometry { get; set; }
    }
}
