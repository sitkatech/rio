using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("DisadvantagedCommunity")]
    [Index(nameof(DisadvantagedCommunityName), nameof(LSADCode), Name = "AK_DisadvantagedCommunity_DisadvantagedCommunityName_LSADCode", IsUnique = true)]
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
        [InverseProperty("DisadvantagedCommunities")]
        public virtual DisadvantagedCommunityStatus DisadvantagedCommunityStatus { get; set; }
    }
}
