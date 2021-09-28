using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class spatial_ref_sys
    {
        [Key]
        public int srid { get; set; }
        [StringLength(256)]
        public string auth_name { get; set; }
        public int? auth_srid { get; set; }
        [StringLength(2048)]
        public string srtext { get; set; }
        [StringLength(2048)]
        public string proj4text { get; set; }
    }
}
