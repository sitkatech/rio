﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vParcelOwnership
    {
        public int UserParcelID { get; set; }
        public int? UserID { get; set; }
        public int ParcelID { get; set; }
        [StringLength(214)]
        public string OwnerName { get; set; }
        public int? EffectiveYear { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? SaleDate { get; set; }
        [StringLength(500)]
        public string Note { get; set; }
        public long? RowNumber { get; set; }
    }
}
