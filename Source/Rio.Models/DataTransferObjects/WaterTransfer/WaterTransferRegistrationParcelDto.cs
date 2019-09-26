using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferRegistrationParcelDto
    {
        [Required]
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        [Required]
        public int AcreFeetTransferred { get; set; }
    }
}