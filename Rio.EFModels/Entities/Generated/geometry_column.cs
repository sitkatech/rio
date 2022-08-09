﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    public partial class geometry_column
    {
        [Key]
        [StringLength(128)]
        [Unicode(false)]
        public string f_table_catalog { get; set; }
        [Key]
        [StringLength(128)]
        [Unicode(false)]
        public string f_table_schema { get; set; }
        [Key]
        [StringLength(256)]
        [Unicode(false)]
        public string f_table_name { get; set; }
        [Key]
        [StringLength(256)]
        [Unicode(false)]
        public string f_geometry_column { get; set; }
        public int coord_dimension { get; set; }
        public int srid { get; set; }
        [Required]
        [StringLength(30)]
        [Unicode(false)]
        public string geometry_type { get; set; }
    }
}
