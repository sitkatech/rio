using System;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelChangeOwnerDto
    {
        [Required]
        public int ParcelID { get; set; }
        public int? AccountID { get; set; }
        [Required]
        public int EffectiveWaterYearID { get; set; }
        [Required]
        public bool ApplyToSubsequentYears { get; set; }
    }
}