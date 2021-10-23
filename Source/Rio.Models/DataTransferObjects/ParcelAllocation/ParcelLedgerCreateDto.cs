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
        public DateTime EffectiveDate { get; set; }
        [Required]
        public int TransactionTypeID { get; set; }
        [Required]
        public decimal TransactionAmount { get; set; }
        public int? WaterTypeID { get; set; }
        public string Comment { get; set; }
    }
}