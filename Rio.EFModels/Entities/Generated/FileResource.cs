using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Table("FileResource")]
    [Index("FileResourceGUID", Name = "AK_FileResource_FileResourceGUID", IsUnique = true)]
    public partial class FileResource
    {
        [Key]
        public int FileResourceID { get; set; }
        public int FileResourceMimeTypeID { get; set; }
        [Required]
        [StringLength(255)]
        [Unicode(false)]
        public string OriginalBaseFilename { get; set; }
        [Required]
        [StringLength(255)]
        [Unicode(false)]
        public string OriginalFileExtension { get; set; }
        public Guid FileResourceGUID { get; set; }
        [Required]
        public byte[] FileResourceData { get; set; }
        public int CreateUserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [ForeignKey("CreateUserID")]
        [InverseProperty("FileResources")]
        public virtual User CreateUser { get; set; }
    }
}
