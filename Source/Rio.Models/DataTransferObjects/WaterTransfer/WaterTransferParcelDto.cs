using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferParcelDto
    {
        [Required]
        public int ParcelID { get; set; }
        public string ParcelNumber { get; set; }
        [Required]
        public int AcreFeetTransferred { get; set; }
        [Required]
        public int WaterTransferTypeID { get; set; }
    }
}