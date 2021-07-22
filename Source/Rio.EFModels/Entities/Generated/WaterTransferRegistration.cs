using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("WaterTransferRegistration")]
    public partial class WaterTransferRegistration
    {
        public WaterTransferRegistration()
        {
            WaterTransferRegistrationParcels = new HashSet<WaterTransferRegistrationParcel>();
        }

        [Key]
        public int WaterTransferRegistrationID { get; set; }
        public int WaterTransferID { get; set; }
        public int WaterTransferTypeID { get; set; }
        public int AccountID { get; set; }
        public int WaterTransferRegistrationStatusID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StatusDate { get; set; }

        [ForeignKey(nameof(AccountID))]
        [InverseProperty("WaterTransferRegistrations")]
        public virtual Account Account { get; set; }
        [ForeignKey(nameof(WaterTransferID))]
        [InverseProperty("WaterTransferRegistrations")]
        public virtual WaterTransfer WaterTransfer { get; set; }
        [ForeignKey(nameof(WaterTransferRegistrationStatusID))]
        [InverseProperty("WaterTransferRegistrations")]
        public virtual WaterTransferRegistrationStatus WaterTransferRegistrationStatus { get; set; }
        [ForeignKey(nameof(WaterTransferTypeID))]
        [InverseProperty("WaterTransferRegistrations")]
        public virtual WaterTransferType WaterTransferType { get; set; }
        [InverseProperty(nameof(WaterTransferRegistrationParcel.WaterTransferRegistration))]
        public virtual ICollection<WaterTransferRegistrationParcel> WaterTransferRegistrationParcels { get; set; }
    }
}
