using System;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Parcel
{
    public class ParcelChangeOwnerDto
    {
        [Required]
        public int ParcelID { get; set; }
        public string OwnerName { get; set; }
        public int? AccountID { get; set; }
        public int? EffectiveYear { get; set; }
        [Required]
        public DateTime SaleDate { get; set; }
        public string Note { get; set; }
    }
}