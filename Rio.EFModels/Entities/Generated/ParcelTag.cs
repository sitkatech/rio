using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("ParcelTag")]
    public partial class ParcelTag
    {
        [Key]
        public int ParcelTagID { get; set; }
        public int ParcelID { get; set; }
        public int TagID { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelTags")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("TagID")]
        [InverseProperty("ParcelTags")]
        public virtual Tag Tag { get; set; }
    }
}
