using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class OrganizationType
    {
        public OrganizationType()
        {
            Organization = new HashSet<Organization>();
        }

        [Column("OrganizationTypeID")]
        public int OrganizationTypeId { get; set; }
        [Required]
        [StringLength(200)]
        public string OrganizationTypeName { get; set; }
        [Required]
        [StringLength(100)]
        public string OrganizationTypeAbbreviation { get; set; }
        [Required]
        [StringLength(10)]
        public string LegendColor { get; set; }
        public bool IsDefaultOrganizationType { get; set; }

        [InverseProperty("OrganizationType")]
        public virtual ICollection<Organization> Organization { get; set; }
    }
}
