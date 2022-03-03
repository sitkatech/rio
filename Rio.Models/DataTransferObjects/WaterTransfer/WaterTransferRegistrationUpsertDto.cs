using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationUpsertDto
    {
        [Required]
        public int WaterTransferTypeID { get; set; }
        [Required]
        public int AccountID { get; set; }
        public int? WaterTransferRegistrationStatusID { get; set; }
        public DateTime? StatusDate { get; set; }
        public List<WaterTransferRegistrationParcelUpsertDto> WaterTransferRegistrationParcels { get; set; }

        // Used for cancellations only
        public int? UserID { get; set; }
    }
}