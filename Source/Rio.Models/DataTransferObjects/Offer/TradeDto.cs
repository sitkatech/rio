using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.Models.DataTransferObjects.Offer
{
    public class TradeDto
    {
        public int TradeID { get; set; }
        public string TradeNumber { get; set; }
        public AccountDto CreateAccount { get; set; }
        public TradeStatusDto TradeStatus { get; set; }
        public PostingDto Posting { get; set; }
    }
}