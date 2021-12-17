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
        public DateTime EffectiveDate { get; set; }
        [Required]
        public int TransactionTypeID { get; set; }
        public int? WaterTypeID { get; set; }
        [Required]
        public decimal TransactionAmount { get; set; }
        public string? UserComment { get; set; }
    }
}