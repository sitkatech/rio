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
        public decimal Price { get; set; }
        public string PostingDescription { get; set; }

        public UserDto CreateUser { get; set; }

        public static string GetListRoute => "postings";

        public static string GetSingleRouteByPostingID(int postingID)
        {
            return $"postings/{postingID}";
        }
    }
}
