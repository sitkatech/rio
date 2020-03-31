using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class ScenarioWells
    {
        [Key]
        public int ogr_fid { get; set; }
        [Column(TypeName = "geometry")]
        public Geometry ogr_geometry { get; set; }
        public double? objectid_1 { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? objectid { get; set; }
        [StringLength(254)]
        public string wcrnumber { get; set; }
        [StringLength(254)]
        public string legacylogn { get; set; }
        [StringLength(254)]
        public string countyname { get; set; }
        [StringLength(254)]
        public string localpermi { get; set; }
        [StringLength(254)]
        public string permitdate { get; set; }
        [StringLength(254)]
        public string permitnumb { get; set; }
        [StringLength(254)]
        public string planneduse { get; set; }
        [StringLength(254)]
        public string recordtype { get; set; }
        [StringLength(254)]
        public string methodofde { get; set; }
        [StringLength(254)]
        public string llaccuracy { get; set; }
        [StringLength(254)]
        public string apn { get; set; }
        [StringLength(24)]
        public string dateworken { get; set; }
        [StringLength(254)]
        public string totaldrill { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? totalcompl { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? topofperfo { get; set; }
        [Column(TypeName = "numeric(10, 0)")]
        public decimal? bottomofpe { get; set; }
        [StringLength(254)]
        public string mtrs { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? ddlong { get; set; }
        [Column(TypeName = "numeric(24, 15)")]
        public decimal? ddlat { get; set; }
    }
}
