using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Table("FileResource")]
    [Index(nameof(FileResourceGUID), Name = "AK_FileResource_FileResourceGUID", IsUnique = true)]
    public partial class FileResource
    {
        public FileResource()
        {
            ParcelAllocationHistories = new HashSet<ParcelAllocationHistory>();
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
        [InverseProperty(nameof(User.FileResources))]
        public virtual User CreateUser { get; set; }
        [ForeignKey(nameof(FileResourceMimeTypeID))]
        [InverseProperty("FileResources")]
        public virtual FileResourceMimeType FileResourceMimeType { get; set; }
        [InverseProperty(nameof(ParcelAllocationHistory.FileResource))]
        public virtual ICollection<ParcelAllocationHistory> ParcelAllocationHistories { get; set; }
    }
}
