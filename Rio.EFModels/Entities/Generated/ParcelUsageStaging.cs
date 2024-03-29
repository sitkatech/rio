﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("ParcelUsageStaging")]
    public partial class ParcelUsageStaging
    {
        [Key]
        public int ParcelUsageStagingID { get; set; }
        public int? ParcelID { get; set; }
        [Required]
        [StringLength(20)]
        [Unicode(false)]
        public string ParcelNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ReportedDate { get; set; }
        [Column(TypeName = "decimal(20, 4)")]
        public decimal ReportedValue { get; set; }
        [Column(TypeName = "decimal(20, 4)")]
        public decimal ReportedValueInAcreFeet { get; set; }
        public int ParcelUsageFileUploadID { get; set; }
        public int UserID { get; set; }

        [ForeignKey("ParcelID")]
        [InverseProperty("ParcelUsageStagings")]
        public virtual Parcel Parcel { get; set; }
        [ForeignKey("ParcelUsageFileUploadID")]
        [InverseProperty("ParcelUsageStagings")]
        public virtual ParcelUsageFileUpload ParcelUsageFileUpload { get; set; }
        [ForeignKey("UserID")]
        [InverseProperty("ParcelUsageStagings")]
        public virtual User User { get; set; }
    }
}
