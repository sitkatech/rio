using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransfer
    {
        public int WaterTransferID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime TransferDate { get; set; }
        [Column(TypeName = "decimal(10, 4)")]
        public decimal? AcreFeetTransferred { get; set; }
        public int TransferringUserID { get; set; }
        public int ReceivingUserID { get; set; }
        public int? OfferID { get; set; }
        [StringLength(2000)]
        public string Notes { get; set; }

        [ForeignKey("OfferID")]
        [InverseProperty("WaterTransfer")]
        public virtual Offer Offer { get; set; }
        [ForeignKey("ReceivingUserID")]
        [InverseProperty("WaterTransferReceivingUser")]
        public virtual User ReceivingUser { get; set; }
        [ForeignKey("TransferringUserID")]
        [InverseProperty("WaterTransferTransferringUser")]
        public virtual User TransferringUser { get; set; }
    }
}
