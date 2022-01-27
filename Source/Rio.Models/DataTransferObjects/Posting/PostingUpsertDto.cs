using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Posting
{
    public class PostingUpsertDto
    {
        [Display(Name = "Type")]
        public int? PostingTypeID { get; set; }
        [Required]
        [Range(1, 100000000)]
        public int? Quantity { get; set; }
        [Required]
        [Range(1, 100000000)]
        public decimal? Price { get; set; }

        public string PostingDescription { get; set; }
        [Display(Name = "Account")]
        [Required]
        public int? CreateAccountID { get; set; }
        [Required]
        public int CreateUserID { get; set; }
    }
}