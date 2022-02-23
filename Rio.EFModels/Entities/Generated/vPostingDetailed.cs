using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Rio.EFModels.Entities
{
    [Keyless]
    public partial class vPostingDetailed
    {
        public int PostingID { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime PostingDate { get; set; }
        public int PostingTypeID { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingTypeDisplayName { get; set; }
        public int PostingStatusID { get; set; }
        [Required]
        [StringLength(100)]
        public string PostingStatusDisplayName { get; set; }
        public int? PostedByUserID { get; set; }
        [StringLength(100)]
        public string PostedByFirstName { get; set; }
        [StringLength(100)]
        public string PostedByLastName { get; set; }
        [StringLength(255)]
        public string PostedByEmail { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public int PostedByAccountID { get; set; }
        [StringLength(255)]
        public string PostedByAccountName { get; set; }
        public int? NumberOfOffers { get; set; }
    }
}
