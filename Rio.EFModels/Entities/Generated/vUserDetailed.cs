using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Rio.EFModels.Entities
{
    [Keyless]
    public partial class vUserDetailed
    {
        public int UserID { get; set; }
        public Guid? UserGuid { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string LastName { get; set; }
        [Required]
        [StringLength(255)]
        [Unicode(false)]
        public string Email { get; set; }
        [StringLength(128)]
        [Unicode(false)]
        public string LoginName { get; set; }
        [StringLength(30)]
        [Unicode(false)]
        public string Phone { get; set; }
        [StringLength(100)]
        [Unicode(false)]
        public string Company { get; set; }
        public bool ReceiveSupportEmails { get; set; }
        public int RoleID { get; set; }
        [Required]
        [StringLength(100)]
        [Unicode(false)]
        public string RoleDisplayName { get; set; }
        public bool? HasActiveTrades { get; set; }
        public int? AcreFeetOfWaterPurchased { get; set; }
        public int? AcreFeetOfWaterSold { get; set; }
    }
}
