using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class User
    {
        public User()
        {
            AuditLog = new HashSet<AuditLog>();
            FileResource = new HashSet<FileResource>();
            Organization = new HashSet<Organization>();
            SupportRequestLog = new HashSet<SupportRequestLog>();
        }

        [Column("UserID")]
        public int UserID { get; set; }
        public Guid UserGuid { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(30)]
        public string Phone { get; set; }
        [Column("RoleID")]
        public int RoleID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastActivityDate { get; set; }
        public bool IsActive { get; set; }
        [Column("OrganizationID")]
        public int? OrganizationID { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        [Required]
        [StringLength(128)]
        public string LoginName { get; set; }

        [ForeignKey("OrganizationID")]
        [InverseProperty("User")]
        public virtual Organization OrganizationNavigation { get; set; }
        [ForeignKey("RoleID")]
        [InverseProperty("User")]
        public virtual Role Role { get; set; }
        [InverseProperty("User")]
        public virtual ICollection<AuditLog> AuditLog { get; set; }
        [InverseProperty("CreateUser")]
        public virtual ICollection<FileResource> FileResource { get; set; }
        [InverseProperty("PrimaryContactUser")]
        public virtual ICollection<Organization> Organization { get; set; }
        [InverseProperty("RequestUser")]
        public virtual ICollection<SupportRequestLog> SupportRequestLog { get; set; }
    }
}
