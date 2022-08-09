using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("DisadvantagedCommunityStatus")]
    [Index("DisadvantagedCommunityStatusName", Name = "AK_DisadvantagedCommunityStatus_DisadvantagedCommunityStatusName", IsUnique = true)]
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
        [Unicode(false)]
        public string DisadvantagedCommunityStatusName { get; set; }
        [Required]
        [StringLength(10)]
        [Unicode(false)]
        public string GeoServerLayerColor { get; set; }

        [InverseProperty("DisadvantagedCommunityStatus")]
        public virtual ICollection<DisadvantagedCommunity> DisadvantagedCommunities { get; set; }
    }
}
