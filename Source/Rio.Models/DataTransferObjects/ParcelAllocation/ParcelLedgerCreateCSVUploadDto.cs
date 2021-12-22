using System;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.ParcelAllocation
{
    public class ParcelLedgerCreateCSVUploadDto
    {
        [Required]
        public DateTime EffectiveDate { get; set; }
        [Required]
        public int WaterTypeID { get; set; }
    }
}