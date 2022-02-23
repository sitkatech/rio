using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("WaterTransferRegistrationParcel")]
    public partial class WaterTransferRegistrationParcel
    {
        [Key]
        public int WaterTransferRegistrationParcelID { get; set; }
        public int WaterTransferRegistrationID { get; set; }
        public int ParcelID { get; set; }
        public int AcreFeetTransferred { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("WaterTransferRegistrationParcels")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(WaterTransferRegistrationID))]
        [InverseProperty("WaterTransferRegistrationParcels")]
        public virtual WaterTransferRegistration WaterTransferRegistration { get; set; }
    }
}
