using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rio.EFModels.Entities
{
    public partial class vUserDetailed
    {
        public int UserID { get; set; }
        public Guid? UserGuid { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [StringLength(255)]
        public string Email { get; set; }
        [StringLength(128)]
        public string LoginName { get; set; }
        [StringLength(30)]
        public string Phone { get; set; }
        [StringLength(100)]
        public string Company { get; set; }
        public int RoleID { get; set; }
        [Required]
        [StringLength(100)]
        public string RoleDisplayName { get; set; }
        public bool? HasActiveTrades { get; set; }
        public int? AcreFeetOfWaterPurchased { get; set; }
        public int? AcreFeetOfWaterSold { get; set; }
    }
}
