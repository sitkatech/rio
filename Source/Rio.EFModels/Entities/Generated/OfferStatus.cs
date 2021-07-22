using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("OfferStatus")]
    [Index(nameof(OfferStatusDisplayName), Name = "AK_OfferStatus_OfferStatusDisplayName", IsUnique = true)]
    [Index(nameof(OfferStatusName), Name = "AK_OfferStatus_OfferStatusName", IsUnique = true)]
    public partial class OfferStatus
    {
        public OfferStatus()
        {
            Offers = new HashSet<Offer>();
        }

        [Key]
        public int OfferStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string OfferStatusName { get; set; }
        [Required]
        [StringLength(100)]
        public string OfferStatusDisplayName { get; set; }

        [InverseProperty(nameof(Offer.OfferStatus))]
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
