﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class UserParcel
    {
        [Key]
        public int UserParcelID { get; set; }
        public int UserID { get; set; }
        public int ParcelID { get; set; }

        [ForeignKey(nameof(ParcelID))]
        [InverseProperty("UserParcel")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey(nameof(UserID))]
        [InverseProperty("UserParcel")]
        public virtual User User { get; set; }
    }
}
