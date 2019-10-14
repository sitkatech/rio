using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationDto
    {
        [Required]
        public int WaterTransferTypeID { get; set; }
        [Required]
        public int UserID { get; set; }
        public int? WaterTransferRegistrationStatusID { get; set; }
        public DateTime? StatusDate { get; set; }
        public List<WaterTransferRegistrationParcelDto> WaterTransferRegistrationParcels { get; set; }
    }
}