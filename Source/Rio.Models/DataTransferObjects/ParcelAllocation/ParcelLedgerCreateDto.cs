using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerCreateDto
    {
        [Required]
        public List<string> ParcelNumbers { get; set; }
        [Required]
        [Range(typeof(DateTime), "1/1/2018", "12/31/9999",
            ErrorMessage = "Date must be between 1/1/2018 and 12/31/9999")]
        public DateTime EffectiveDate { get; set; }
        [Required]
        public int TransactionTypeID { get; set; }
        public int? WaterTypeID { get; set; }
        [Required]
        public decimal TransactionAmount { get; set; }
        public string? UserComment { get; set; }
    }
}