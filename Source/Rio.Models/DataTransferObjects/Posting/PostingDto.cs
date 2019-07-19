using System;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Posting
{
    public class PostingDto
    {
        public int PostingID { get; set; }
        public PostingTypeDto PostingType { get; set; }
        public DateTime PostingDate { get; set; }
        public int Quantity { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal Price { get; set; }
        public string PostingDescription { get; set; }

        public UserSimpleDto CreateUser { get; set; }
        public PostingStatusDto PostingStatus { get; set; }
    }
}
