using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GeoAPI.Geometries;

namespace Rio.EFModels.Entities
{
    public partial class Parcel
    {
        public Parcel()
        {
            ParcelAllocation = new HashSet<ParcelAllocation>();
            ParcelMonthlyEvapotranspiration = new HashSet<ParcelMonthlyEvapotranspiration>();
            UserParcel = new HashSet<UserParcel>();
        }

        public int ParcelID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelNumber { get; set; }
        [Required]
        [Column(TypeName = "geometry")]
        public IGeometry ParcelGeometry { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerName { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerAddress { get; set; }
        [Required]
        [StringLength(100)]
        public string OwnerCity { get; set; }
        [Required]
        [StringLength(20)]
        public string OwnerZipCode { get; set; }
        public int ParcelAreaInSquareFeet { get; set; }
        public double ParcelAreaInAcres { get; set; }

        [InverseProperty("Parcel")]
        public virtual ICollection<ParcelAllocation> ParcelAllocation { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<ParcelMonthlyEvapotranspiration> ParcelMonthlyEvapotranspiration { get; set; }
        [InverseProperty("Parcel")]
        public virtual ICollection<UserParcel> UserParcel { get; set; }
    }
}
