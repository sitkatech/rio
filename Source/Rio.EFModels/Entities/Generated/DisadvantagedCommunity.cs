using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class DisadvantagedCommunity
    {
        [Key]
        public int ogr_fid { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry ogr_geometry { get; set; }
        public double? objectid { get; set; }
        [StringLength(2)]
        public string statefp { get; set; }
        [StringLength(5)]
        public string placefp { get; set; }
        [StringLength(8)]
        public string placens { get; set; }
        [StringLength(7)]
        public string geoid { get; set; }
        [StringLength(100)]
        public string name { get; set; }
        [StringLength(100)]
        public string namelsad { get; set; }
        [StringLength(2)]
        public string lsad { get; set; }
        [StringLength(2)]
        public string classfp { get; set; }
        [StringLength(1)]
        public string pcicbsa { get; set; }
        [StringLength(1)]
        public string pcinecta { get; set; }
        [StringLength(5)]
        public string mtfcc { get; set; }
        [StringLength(1)]
        public string funcstat { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? aland { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? awater { get; set; }
        [StringLength(11)]
        public string intptlat { get; set; }
        [StringLength(12)]
        public string intptlon { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? pop { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? mhhi { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? white_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? asian_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? afamer_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? hislat_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? nhpi_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? aind_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? other_ct { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? more2_ct { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? white_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? asian_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? afamer_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? hislat_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? nhpi_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? aind_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? other_per { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? more2_per { get; set; }
        [StringLength(50)]
        public string dac_status { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? shape_leng { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? shape_area { get; set; }
    }
}
