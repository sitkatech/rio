using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferParcel
    {
        public int WaterTransferParcelID { get; set; }
        public int WaterTransferID { get; set; }
        public int ParcelID { get; set; }
        public int WaterTransferTypeID { get; set; }
        public int AcreFeetTransferred { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("WaterTransferParcel")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("WaterTransferID")]
        [InverseProperty("WaterTransferParcel")]
        public virtual WaterTransfer WaterTransfer { get; set; }
        [ForeignKey("WaterTransferTypeID")]
        [InverseProperty("WaterTransferParcel")]
        public virtual WaterTransferType WaterTransferType { get; set; }
    }
}
