using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class OfferUpsertDto
    {
        public int? TradeID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string OfferNotes { get; set; }
        public int OfferStatusID { get; set; }
    }
}