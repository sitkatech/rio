using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.WaterTransfer
{
    public class WaterTransferConfirmDto
    {
        [Required]
        public int WaterTransferType { get; set; }
        [Required]
        public int ConfirmingUserID { get; set; }
    }
}