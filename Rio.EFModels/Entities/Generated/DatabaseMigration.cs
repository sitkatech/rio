using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("DatabaseMigration")]
    [Index(nameof(ReleaseScriptFileName), Name = "AK_DatabaseMigration_ReleaseScriptFileName", IsUnique = true)]
    public partial class DatabaseMigration
    {
        [Key]
        public int DatabaseMigrationNumber { get; set; }
        [Required]
        [StringLength(500)]
        public string ReleaseScriptFileName { get; set; }
        public DateTime DateMigrated { get; set; }
        [Required]
        [StringLength(200)]
        public string MigrationAuthorName { get; set; }
        public string MigrationReason { get; set; }
    }
}
