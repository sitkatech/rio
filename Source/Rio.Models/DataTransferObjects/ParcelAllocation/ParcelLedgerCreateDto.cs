using System;
using System.ComponentModel.DataAnnotations;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerCreateDto
    {
        [Required]
        public int ParcelID { get; set; }

        [Required]
        [Range(typeof(DateTime), "1/1/2018", "12/31/9999",
            ErrorMessage = "Date must be between {1} and {2}")]
        public DateTime EffectiveDate { get; set; }
        [Required]
        public int TransactionTypeID { get; set; }
        public int? WaterTypeID { get; set; }
        [Required]
        public bool IsWithdrawal { get; set; }
        [Required]
        [Range(0, 1934,
            ErrorMessage = "Please enter a quantity between {1} and {2}. If you'd like to apply a negative correction, enter a positive quantity and select 'Withdrawal'.")]
        public decimal TransactionAmount { get; set; }
        public string? UserComment { get; set; }
    }
}