using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("Parcel")]
    [Index(nameof(ParcelNumber), Name = "AK_Parcel_ParcelNumber", IsUnique = true)]
    [Index(nameof(ParcelGeometry), Name = "SPATIAL_Parcel_ParcelGeometry")]
    public partial class Parcel
    {
        public Parcel()
        {
            AccountParcelWaterYears = new HashSet<AccountParcelWaterYear>();
            AccountReconciliations = new HashSet<AccountReconciliation>();
            ParcelLedgers = new HashSet<ParcelLedger>();
            ParcelTags = new HashSet<ParcelTag>();
            WaterTransferRegistrationParcels = new HashSet<WaterTransferRegistrationParcel>();
        }

        [Key]
        public int ParcelID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public Geometry ParcelGeometry { get; set; }
        public int ParcelAreaInSquareFeet { get; set; }
        public double ParcelAreaInAcres { get; set; }
        public int ParcelStatusID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? InactivateDate { get; set; }

        [InverseProperty(nameof(AccountParcelWaterYear.Parcel))]
        public virtual ICollection<AccountParcelWaterYear> AccountParcelWaterYears { get; set; }
        [InverseProperty(nameof(AccountReconciliation.Parcel))]
        public virtual ICollection<AccountReconciliation> AccountReconciliations { get; set; }
        [InverseProperty(nameof(ParcelLedger.Parcel))]
        public virtual ICollection<ParcelLedger> ParcelLedgers { get; set; }
        [InverseProperty(nameof(ParcelTag.Parcel))]
        public virtual ICollection<ParcelTag> ParcelTags { get; set; }
        [InverseProperty(nameof(WaterTransferRegistrationParcel.Parcel))]
        public virtual ICollection<WaterTransferRegistrationParcel> WaterTransferRegistrationParcels { get; set; }
    }
}
