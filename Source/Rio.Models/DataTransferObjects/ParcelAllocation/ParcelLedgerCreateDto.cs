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
        public string ParcelNumber { get; set; }
        [Required]
        [Range(typeof(DateTime), "1/1/2018", "12/31/9999",
            ErrorMessage = "Date must be between 1/1/2018 and 12/31/9999")]
        public DateTime EffectiveDate { get; set; }
        [Required]
        public int TransactionTypeID { get; set; }
        public int? WaterTypeID { get; set; }
        [Required]
        public bool IsWithdrawal { get; set; }
        [Required]
        [Range(0, 1934,
            ErrorMessage = "Please enter a quantity greater than 0. If you'd like to apply a negative correction, enter a positive quantity and select 'Withdrawal'.")]
        public decimal TransactionAmount { get; set; }
        public string? UserComment { get; set; }
    }
}