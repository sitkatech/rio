using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistrationParcel
    {
        public int WaterTransferRegistrationParcelID { get; set; }
        public int WaterTransferRegistrationID { get; set; }
        public int ParcelID { get; set; }
        public int AcreFeetTransferred { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("WaterTransferRegistrationParcel")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("WaterTransferRegistrationID")]
        [InverseProperty("WaterTransferRegistrationParcel")]
        public virtual WaterTransferRegistration WaterTransferRegistration { get; set; }
    }
}
