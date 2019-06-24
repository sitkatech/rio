using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class SupportRequestLog
    {
        [Column("SupportRequestLogID")]
        public int SupportRequestLogId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime RequestDate { get; set; }
        [Required]
        [StringLength(200)]
        public string RequestUserName { get; set; }
        [Required]
        [StringLength(256)]
        public string RequestUserEmail { get; set; }
        [Column("RequestUserID")]
        public int? RequestUserId { get; set; }
        [Column("SupportRequestTypeID")]
        public int SupportRequestTypeId { get; set; }
        [Required]
        [StringLength(2000)]
        public string RequestDescription { get; set; }
        [StringLength(500)]
        public string RequestUserOrganization { get; set; }
        [StringLength(50)]
        public string RequestUserPhone { get; set; }

        [ForeignKey("RequestUserId")]
        [InverseProperty("SupportRequestLog")]
        public virtual User RequestUser { get; set; }
        [ForeignKey("SupportRequestTypeId")]
        [InverseProperty("SupportRequestLog")]
        public virtual SupportRequestType SupportRequestType { get; set; }
    }
}
