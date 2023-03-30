using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("ParcelUsageFileUpload")]
    public partial class ParcelUsageFileUpload
    {
        public ParcelUsageFileUpload()
        {
            ParcelUsageStagings = new HashSet<ParcelUsageStaging>();
        }

        [Key]
        public int ParcelUsageFileUploadID { get; set; }
        public int UserID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string UploadedFileName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UploadDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PublishDate { get; set; }
        public int MatchedRecordCount { get; set; }
        public int UnmatchedParcelNumberCount { get; set; }
        public int NullParcelNumberCount { get; set; }

        [ForeignKey("UserID")]
        [InverseProperty("ParcelUsageFileUploads")]
        public virtual User User { get; set; }
        [InverseProperty("ParcelUsageFileUpload")]
        public virtual ICollection<ParcelUsageStaging> ParcelUsageStagings { get; set; }
    }
}
