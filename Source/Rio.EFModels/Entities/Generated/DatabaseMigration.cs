using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class DatabaseMigration
    {
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
