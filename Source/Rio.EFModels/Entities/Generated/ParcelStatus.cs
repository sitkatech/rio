using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelStatus")]
    [Index(nameof(ParcelStatusDisplayName), Name = "AK_ParcelStatus_ParcelStatusDisplayName", IsUnique = true)]
    [Index(nameof(ParcelStatusName), Name = "AK_ParcelStatus_ParcelStatusName", IsUnique = true)]
    public partial class ParcelStatus
    {
        public ParcelStatus()
        {
            Parcels = new HashSet<Parcel>();
        }

        [Key]
        public int ParcelStatusID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelStatusName { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelStatusDisplayName { get; set; }

        [InverseProperty(nameof(Parcel.ParcelStatus))]
        public virtual ICollection<Parcel> Parcels { get; set; }
    }
}
