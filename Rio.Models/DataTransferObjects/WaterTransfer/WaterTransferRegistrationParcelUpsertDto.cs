﻿using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationParcelUpsertDto
    {
        [Required]
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        [Required]
        public int AcreFeetTransferred { get; set; }
        public double? ParcelAreaInAcres { get; set; }
    }
}