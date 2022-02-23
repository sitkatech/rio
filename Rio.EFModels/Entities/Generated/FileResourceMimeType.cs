using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("FileResourceMimeType")]
    [Index(nameof(FileResourceMimeTypeDisplayName), Name = "AK_FileResourceMimeType_FileResourceMimeTypeDisplayName", IsUnique = true)]
    [Index(nameof(FileResourceMimeTypeName), Name = "AK_FileResourceMimeType_FileResourceMimeTypeName", IsUnique = true)]
    public partial class FileResourceMimeType
    {
        public FileResourceMimeType()
        {
            FileResources = new HashSet<FileResource>();
        }

        [Key]
        public int FileResourceMimeTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string FileResourceMimeTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string FileResourceMimeTypeDisplayName { get; set; }
        [Required]
        [StringLength(100)]
        public string FileResourceMimeTypeContentTypeName { get; set; }
        [StringLength(100)]
        public string FileResourceMimeTypeIconSmallFilename { get; set; }
        [StringLength(100)]
        public string FileResourceMimeTypeIconNormalFilename { get; set; }

        [InverseProperty(nameof(FileResource.FileResourceMimeType))]
        public virtual ICollection<FileResource> FileResources { get; set; }
    }
}
