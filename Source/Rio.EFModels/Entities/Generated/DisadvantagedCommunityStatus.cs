using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class DisadvantagedCommunityStatus
    {
        public DisadvantagedCommunityStatus()
        {
            DisadvantagedCommunity = new HashSet<DisadvantagedCommunity>();
        }

        [Key]
        public int DisadvantagedCommunityStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string DisadvantagedCommunityStatusName { get; set; }
        [Required]
        [StringLength(10)]
        public string GeoServerLayerColor { get; set; }

        [InverseProperty("DisadvantagedCommunityStatus")]
        public virtual ICollection<DisadvantagedCommunity> DisadvantagedCommunity { get; set; }
    }
}
