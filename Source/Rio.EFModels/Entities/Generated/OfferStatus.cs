using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OfferStatus
    {
        public OfferStatus()
        {
            Offer = new HashSet<Offer>();
        }

        [Key]
        public int OfferStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string OfferStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string OfferStatusDisplayName { get; set; }

        [InverseProperty("OfferStatus")]
        public virtual ICollection<Offer> Offer { get; set; }
    }
}
