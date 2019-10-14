using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class WaterTransferRegistration
    {
        public WaterTransferRegistration()
        {
            WaterTransferRegistrationParcel = new HashSet<WaterTransferRegistrationParcel>();
        }

        public int WaterTransferRegistrationID { get; set; }
        public int WaterTransferID { get; set; }
        public int WaterTransferTypeID { get; set; }
        public int UserID { get; set; }
        public int WaterTransferRegistrationStatusID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StatusDate { get; set; }

        [ForeignKey("UserID")]
        [InverseProperty("WaterTransferRegistration")]
        public virtual User User { get; set; }
        [ForeignKey("WaterTransferID")]
        [InverseProperty("WaterTransferRegistration")]
        public virtual WaterTransfer WaterTransfer { get; set; }
        [ForeignKey("WaterTransferRegistrationStatusID")]
        [InverseProperty("WaterTransferRegistration")]
        public virtual WaterTransferRegistrationStatus WaterTransferRegistrationStatus { get; set; }
        [ForeignKey("WaterTransferTypeID")]
        [InverseProperty("WaterTransferRegistration")]
        public virtual WaterTransferType WaterTransferType { get; set; }
        [InverseProperty("WaterTransferRegistration")]
        public virtual ICollection<WaterTransferRegistrationParcel> WaterTransferRegistrationParcel { get; set; }
    }
}
