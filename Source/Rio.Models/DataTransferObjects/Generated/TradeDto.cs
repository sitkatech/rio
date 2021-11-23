//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Trade]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class TradeDto
    {
        public int TradeID { get; set; }
        public PostingDto Posting { get; set; }
        public DateTime TradeDate { get; set; }
        public TradeStatusDto TradeStatus { get; set; }
        public AccountDto CreateAccount { get; set; }
        public string TradeNumber { get; set; }
    }

    public partial class TradeSimpleDto
    {
        public int TradeID { get; set; }
        public int PostingID { get; set; }
        public DateTime TradeDate { get; set; }
        public int TradeStatusID { get; set; }
        public int CreateAccountID { get; set; }
        public string TradeNumber { get; set; }
    }

}