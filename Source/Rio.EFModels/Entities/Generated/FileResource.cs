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
            RioPageImage = new HashSet<RioPageImage>();
            FieldDefinitionDataImage = new HashSet<FieldDefinitionDataImage>();
            Organization = new HashSet<Organization>();
        }

        [Column("FileResourceID")]
        public int FileResourceId { get; set; }
        [Column("FileResourceMimeTypeID")]
        public int FileResourceMimeTypeId { get; set; }
        [Required]
        [StringLength(255)]
        public string OriginalBaseFilename { get; set; }
        [Required]
        [StringLength(255)]
        public string OriginalFileExtension { get; set; }
        [Column("FileResourceGUID")]
        public Guid FileResourceGuid { get; set; }
        [Required]
        public byte[] FileResourceData { get; set; }
        [Column("CreateUserID")]
        public int CreateUserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }

        [ForeignKey("CreateUserId")]
        [InverseProperty("FileResource")]
        public virtual User CreateUser { get; set; }
        [ForeignKey("FileResourceMimeTypeId")]
        [InverseProperty("FileResource")]
        public virtual FileResourceMimeType FileResourceMimeType { get; set; }
        [InverseProperty("FileResource")]
        public virtual ICollection<RioPageImage> RioPageImage { get; set; }
        [InverseProperty("FileResource")]
        public virtual ICollection<FieldDefinitionDataImage> FieldDefinitionDataImage { get; set; }
        [InverseProperty("LogoFileResource")]
        public virtual ICollection<Organization> Organization { get; set; }
    }
}
