using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public Parcel()
        {
            AccountParcelWaterYear = new HashSet<AccountParcelWaterYear>();
            AccountReconciliation = new HashSet<AccountReconciliation>();
            ParcelAllocation = new HashSet<ParcelAllocation>();
            ParcelMonthlyEvapotranspiration = new HashSet<ParcelMonthlyEvapotranspiration>();
            WaterTransferRegistrationParcel = new HashSet<WaterTransferRegistrationParcel>();
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

        [ForeignKey(nameof(ParcelStatusID))]
        [InverseProperty("Parcel")]
        public virtual ParcelStatus ParcelStatus { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<AccountParcelWaterYear> AccountParcelWaterYear { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<AccountReconciliation> AccountReconciliation { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<ParcelAllocation> ParcelAllocation { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<ParcelMonthlyEvapotranspiration> ParcelMonthlyEvapotranspiration { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<WaterTransferRegistrationParcel> WaterTransferRegistrationParcel { get; set; }
    }
}
