using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class OfferUpdateStatusDto
    {
        [Required]
        public int OfferStatusID { get; set; }
    }
}