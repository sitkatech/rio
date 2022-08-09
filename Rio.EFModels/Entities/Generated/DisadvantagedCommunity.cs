using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    [Table("DisadvantagedCommunity")]
    [Index("DisadvantagedCommunityName", "LSADCode", Name = "AK_DisadvantagedCommunity_DisadvantagedCommunityName_LSADCode", IsUnique = true)]
    public partial class DisadvantagedCommunity
    {
        [Key]
        public int DisadvantagedCommunityID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string DisadvantagedCommunityName { get; set; }
        public int LSADCode { get; set; }
        public int DisadvantagedCommunityStatusID { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry DisadvantagedCommunityGeometry { get; set; }

        [ForeignKey("DisadvantagedCommunityStatusID")]
        [InverseProperty("DisadvantagedCommunities")]
        public virtual DisadvantagedCommunityStatus DisadvantagedCommunityStatus { get; set; }
    }
}
