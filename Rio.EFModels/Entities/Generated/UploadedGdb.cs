using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("UploadedGdb")]
    public partial class UploadedGdb
    {
        [Key]
        public int UploadedGdbID { get; set; }
        public byte[] GdbFileContents { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UploadDate { get; set; }
    }
}
