using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Rio.Models.DataTransferObjects;

namespace Rio.EFModels.Entities;

public static class UserMessages
{
    private static IQueryable<UserMessage> GetUserMessageImpl(RioDbContext dbContext)
    {
        return dbContext.UserMessages
            .Include(x => x.CreateUser)
            .Include(x => x.RecipientUser)
            .AsNoTracking();
    }

    public static List<UserMessageDto> ListByCreatedDate(RioDbContext dbContext, int userID)
    {
        return GetUserMessageImpl(dbContext).Where(x => x.CreateUserID == userID || x.RecipientUserID == userID)
            .OrderByDescending(x => x.CreateDate).Select(x => x.AsDto()).ToList();
    }

    public static UserMessageDto GetByUserMessageID(RioDbContext dbContext, int userMessageID)
    {
        return GetUserMessageImpl(dbContext).Single(x => x.UserMessageID == userMessageID).AsDto();
    }

    public static void CreateNewMessageFromSimple(RioDbContext dbContext, UserMessageSimpleDto userMessageSimpleDto)
    {
        UserMessage newMessage = new UserMessage
        {
            CreateUserID = userMessageSimpleDto.CreateUserID,
            RecipientUserID = userMessageSimpleDto.RecipientUserID,
            CreateDate = DateTime.UtcNow,
            Message = userMessageSimpleDto.Message
        };

        dbContext.UserMessages.Add(newMessage);
        dbContext.SaveChanges();
    }
}