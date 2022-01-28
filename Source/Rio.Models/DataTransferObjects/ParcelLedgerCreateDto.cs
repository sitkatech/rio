using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects
{
    public class ParcelLedgerCreateDto
    {
        [Required] 
        public List<string> ParcelNumbers { get; set; }
        [Display(Name = "Effective Date")]
        [Required]
        [RegularExpression(@"^\d{4}\-\d{1,2}\-\d{1,2}$", 
            ErrorMessage = "Effective Date must be entered in YYYY-MM-DD format.")]
        public string? EffectiveDate { get; set; }
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