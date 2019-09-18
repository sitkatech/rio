using System;
using System.ComponentModel.DataAnnotations;

namespace Rio.Models.DataTransferObjects.Posting
{
    public class PostingUpsertDto
    {
        [Required]
        public int PostingTypeID { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }

        public string PostingDescription { get; set; }
        [Required]
        public int CreateUserID { get; set; }
    }
}