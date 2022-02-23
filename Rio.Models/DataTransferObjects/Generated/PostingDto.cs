//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[Posting]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class PostingDto
    {
        public int PostingID { get; set; }
        public PostingTypeDto PostingType { get; set; }
        public DateTime PostingDate { get; set; }
        public AccountDto CreateAccount { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string PostingDescription { get; set; }
        public PostingStatusDto PostingStatus { get; set; }
        public int AvailableQuantity { get; set; }
        public UserDto CreateUser { get; set; }
    }

    public partial class PostingSimpleDto
    {
        public int PostingID { get; set; }
        public int PostingTypeID { get; set; }
        public DateTime PostingDate { get; set; }
        public int CreateAccountID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string PostingDescription { get; set; }
        public int PostingStatusID { get; set; }
        public int AvailableQuantity { get; set; }
        public int? CreateUserID { get; set; }
    }

}