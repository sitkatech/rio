using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class FileResourceMimeType
    {
        public FileResourceMimeType()
        {
            FileResource = new HashSet<FileResource>();
        }

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

        [InverseProperty("FileResourceMimeType")]
        public virtual ICollection<FileResource> FileResource { get; set; }
    }
}
