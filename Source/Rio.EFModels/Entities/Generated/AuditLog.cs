using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class AuditLog
    {
        [Column("AuditLogID")]
        public int AuditLogID { get; set; }
        [Column("UserID")]
        public int UserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime AuditLogDate { get; set; }
        [Column("AuditLogEventTypeID")]
        public int AuditLogEventTypeID { get; set; }
        [Required]
        [StringLength(500)]
        public string TableName { get; set; }
        [Column("RecordID")]
        public int RecordID { get; set; }
        [Required]
        [StringLength(500)]
        public string ColumnName { get; set; }
        public string OriginalValue { get; set; }
        [Required]
        public string NewValue { get; set; }
        public string AuditDescription { get; set; }

        [ForeignKey("AuditLogEventTypeID")]
        [InverseProperty("AuditLog")]
        public virtual AuditLogEventType AuditLogEventType { get; set; }
        [ForeignKey("UserID")]
        [InverseProperty("AuditLog")]
        public virtual User User { get; set; }
    }
}
