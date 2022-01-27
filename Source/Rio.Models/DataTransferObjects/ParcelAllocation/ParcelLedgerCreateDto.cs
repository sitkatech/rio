using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerCreateDto
    {
        [Required] 
        public List<string> ParcelNumbers { get; set; }
        [Display(Name = "Effective Date")]
        [Required]
        public DateTime? EffectiveDate { get; set; }
        public string EffectiveDateString { get; set; }
        [Display(Name = "Water Budget Category")]
        [Required]
        public int? TransactionTypeID { get; set; }
        public int? WaterTypeID { get; set; }
        [Display(Name = "Transaction Amount")]
        [Required]
        public decimal? TransactionAmount { get; set; }
        public string? UserComment { get; set; }
    }
}