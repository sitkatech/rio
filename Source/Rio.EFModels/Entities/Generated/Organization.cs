using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class Organization
    {
        public Organization()
        {
            User = new HashSet<User>();
        }

        [Column("OrganizationID")]
        public int OrganizationId { get; set; }
        public Guid? OrganizationGuid { get; set; }
        [Required]
        [StringLength(200)]
        public string OrganizationName { get; set; }
        [StringLength(50)]
        public string OrganizationShortName { get; set; }
        [Column("PrimaryContactUserID")]
        public int? PrimaryContactUserId { get; set; }
        public bool IsActive { get; set; }
        [StringLength(200)]
        public string OrganizationUrl { get; set; }
        [Column("LogoFileResourceID")]
        public int? LogoFileResourceId { get; set; }
        [Column("OrganizationTypeID")]
        public int OrganizationTypeId { get; set; }

        [ForeignKey("LogoFileResourceId")]
        [InverseProperty("Organization")]
        public virtual FileResource LogoFileResource { get; set; }
        [ForeignKey("OrganizationTypeId")]
        [InverseProperty("Organization")]
        public virtual OrganizationType OrganizationType { get; set; }
        [ForeignKey("PrimaryContactUserId")]
        [InverseProperty("Organization")]
        public virtual User PrimaryContactUser { get; set; }
        [InverseProperty("OrganizationNavigation")]
        public virtual ICollection<User> User { get; set; }
    }
}
