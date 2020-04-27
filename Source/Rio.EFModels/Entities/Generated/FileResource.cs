using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class FileResource
    {
        public FileResource()
        {
            ParcelAllocationHistory = new HashSet<ParcelAllocationHistory>();
        }
        [Key]
        public int FileResourceID { get; set; }
        public int FileResourceMimeTypeID { get; set; }
        [Required]
        [StringLength(255)]
        public string OriginalBaseFilename { get; set; }
        [Required]
        [StringLength(255)]
        public string OriginalFileExtension { get; set; }
        public Guid FileResourceGUID { get; set; }
        [Required]
        public byte[] FileResourceData { get; set; }
        public int CreateUserID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [ForeignKey(nameof(CreateUserID))]
        [InverseProperty(nameof(User.FileResource))]
        public virtual User CreateUser { get; set; }
        [ForeignKey(nameof(FileResourceMimeTypeID))]
        [InverseProperty("FileResource")]
        public virtual FileResourceMimeType FileResourceMimeType { get; set; }
        [InverseProperty("FileResource")]
        public virtual ICollection<ParcelAllocationHistory> ParcelAllocationHistory { get; set; }
    }
}
