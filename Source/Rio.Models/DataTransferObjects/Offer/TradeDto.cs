using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class TradeDto
    {
        public int TradeID { get; set; }
        public string TradeNumber { get; set; }
        public UserSimpleDto CreateUser { get; set; }
        public TradeStatusDto TradeStatus { get; set; }
        public PostingDto Posting { get; set; }
    }
}