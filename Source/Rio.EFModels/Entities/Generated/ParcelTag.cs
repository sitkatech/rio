using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("ParcelTag")]
    public partial class ParcelTag
    {
        [Key]
        public int ParcelTagID { get; set; }
        public int ParcelID { get; set; }
        public int TagID { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("ParcelTags")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(TagID))]
        [InverseProperty("ParcelTags")]
        public virtual Tag Tag { get; set; }
    }
}
