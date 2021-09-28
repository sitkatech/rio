using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("WaterTransfer")]
    public partial class WaterTransfer
    {
        public WaterTransfer()
        {
            WaterTransferRegistrations = new HashSet<WaterTransferRegistration>();
        }

        [Key]
        public int WaterTransferID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TransferDate { get; set; }
        public int AcreFeetTransferred { get; set; }
        public int OfferID { get; set; }
        [StringLength(2000)]
        public string Notes { get; set; }

        [ForeignKey(nameof(OfferID))]
        [InverseProperty("WaterTransfers")]
        public virtual Offer Offer { get; set; }
        [InverseProperty(nameof(WaterTransferRegistration.WaterTransfer))]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistrations { get; set; }
    }
}
