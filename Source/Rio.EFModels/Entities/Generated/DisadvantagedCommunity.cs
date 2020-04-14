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
        public int DisadvantagedCommunityID { get; set; }
        [Required]
        [StringLength(100)]
        public string DisadvantagedCommunityName { get; set; }
        public int LSADCode { get; set; }
        public int DisadvantagedCommunityStatusID { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry DisadvantagedCommunityGeometry { get; set; }

        [ForeignKey(nameof(DisadvantagedCommunityStatusID))]
        [InverseProperty("DisadvantagedCommunity")]
        public virtual DisadvantagedCommunityStatus DisadvantagedCommunityStatus { get; set; }
    }
}
