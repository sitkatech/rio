using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class ParcelStatus
    {
        public ParcelStatus()
        {
            Parcel = new HashSet<Parcel>();
        }

        [Key]
        public int ParcelStatusID { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelStatusName { get; set; }
        [Required]
        [StringLength(20)]
        public string ParcelStatusDisplayName { get; set; }

        [InverseProperty("ParcelStatus")]
        public virtual ICollection<Parcel> Parcel { get; set; }
    }
}
