using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("DisadvantagedCommunityStatus")]
    [Index(nameof(DisadvantagedCommunityStatusName), Name = "AK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusName", IsUnique = true)]
    public partial class DisadvantagedCommunityStatus
    {
        public DisadvantagedCommunityStatus()
        {
            DisadvantagedCommunities = new HashSet<DisadvantagedCommunity>();
        }

        [Key]
        public int DisadvantagedCommunityStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string DisadvantagedCommunityStatusName { get; set; }
        [Required]
        [StringLength(10)]
        public string GeoServerLayerColor { get; set; }

        [InverseProperty(nameof(DisadvantagedCommunity.DisadvantagedCommunityStatus))]
        public virtual ICollection<DisadvantagedCommunity> DisadvantagedCommunities { get; set; }
    }
}
