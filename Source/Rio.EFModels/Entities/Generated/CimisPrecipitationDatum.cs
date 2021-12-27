using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("CimisPrecipitationDatum")]
    public partial class CimisPrecipitationDatum
    {
        [Key]
        public int CimisPrecipitationDatumID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DateMeasured { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal Precipitation { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime LastUpdated { get; set; }
    }
}
