using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers;

[ApiController]
public class UserMessageController : SitkaController<UserMessageController>
{
    public UserMessageController(RioDbContext dbContext, ILogger<UserMessageController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> frescaConfiguration)
        : base(dbContext, logger, keystoneService, frescaConfiguration)
    {
    }

    [HttpPost("user-messages/new")]
    public ActionResult CreateNewUserMessage([FromBody] UserMessageSimpleDto userMessageSimpleDto)
    {
        if (string.IsNullOrWhiteSpace(userMessageSimpleDto.Message))
        {
            return BadRequest($"Message field is required. Please include a message in your request.");
        }

        userMessageSimpleDto.CreateUserID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID;
        UserMessages.CreateNewMessageFromSimple(_dbContext, userMessageSimpleDto);
        return Ok();
    }

    [HttpGet("user-messages/{userMessageID}")]
    public ActionResult<UserMessageDto> GetUserMessageFromUserMessageID([FromRoute] int userMessageID)
    {
        var message = UserMessages.GetByUserMessageID(_dbContext, userMessageID);
        return Ok(message);
    }
}