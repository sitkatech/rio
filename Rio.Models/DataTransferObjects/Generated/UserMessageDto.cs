//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UserMessage]
using System;


namespace Rio.Models.DataTransferObjects
{
    public partial class UserMessageDto
    {
        public int UserMessageID { get; set; }
        public UserDto CreateUser { get; set; }
        public UserDto RecipientUser { get; set; }
        public DateTime CreateDate { get; set; }
        public string Message { get; set; }
    }

    public partial class UserMessageSimpleDto
    {
        public int UserMessageID { get; set; }
        public int CreateUserID { get; set; }
        public int RecipientUserID { get; set; }
        public DateTime CreateDate { get; set; }
        public string Message { get; set; }
    }

}