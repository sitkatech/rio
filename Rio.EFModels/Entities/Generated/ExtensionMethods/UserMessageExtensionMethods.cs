//  IMPORTANT:
//  This file is generated. Your changes will be lost.
//  Use the corresponding partial class for customizations.
//  Source Table: [dbo].[UserMessage]

using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities
{
    public static partial class UserMessageExtensionMethods
    {
        public static UserMessageDto AsDto(this UserMessage userMessage)
        {
            var userMessageDto = new UserMessageDto()
            {
                UserMessageID = userMessage.UserMessageID,
                CreateUser = userMessage.CreateUser.AsDto(),
                RecipientUser = userMessage.RecipientUser.AsDto(),
                CreateDate = userMessage.CreateDate,
                Message = userMessage.Message
            };
            DoCustomMappings(userMessage, userMessageDto);
            return userMessageDto;
        }

        static partial void DoCustomMappings(UserMessage userMessage, UserMessageDto userMessageDto);

        public static UserMessageSimpleDto AsSimpleDto(this UserMessage userMessage)
        {
            var userMessageSimpleDto = new UserMessageSimpleDto()
            {
                UserMessageID = userMessage.UserMessageID,
                CreateUserID = userMessage.CreateUserID,
                RecipientUserID = userMessage.RecipientUserID,
                CreateDate = userMessage.CreateDate,
                Message = userMessage.Message
            };
            DoCustomSimpleDtoMappings(userMessage, userMessageSimpleDto);
            return userMessageSimpleDto;
        }

        static partial void DoCustomSimpleDtoMappings(UserMessage userMessage, UserMessageSimpleDto userMessageSimpleDto);
    }
}