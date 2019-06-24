using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AuditLogEventType
    {
        public AuditLogEventType()
        {
            AuditLog = new HashSet<AuditLog>();
        }

        [Column("AuditLogEventTypeID")]
        public int AuditLogEventTypeId { get; set; }
        [Required]
        [StringLength(100)]
        public string AuditLogEventTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string AuditLogEventTypeDisplayName { get; set; }

        [InverseProperty("AuditLogEventType")]
        public virtual ICollection<AuditLog> AuditLog { get; set; }
    }
}
