using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransfer
    {
        public WaterTransfer()
        {
            WaterTransferRegistration = new HashSet<WaterTransferRegistration>();
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
        [InverseProperty("WaterTransfer")]
        public virtual Offer Offer { get; set; }
        [InverseProperty("WaterTransfer")]
        public virtual ICollection<WaterTransferRegistration> WaterTransferRegistration { get; set; }
    }
}
