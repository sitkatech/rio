using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Controllers
{
    public class UserController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<UserController> _logger;
        private readonly KeystoneService _keystoneService;

        public UserController(RioDbContext dbContext, ILogger<UserController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpPost("/users/invite")]
        [RequiresValidJSONBodyFilter("Could not parse a valid User Invite JSON object from the Request Body.")]
        public IActionResult PostProgramUserInvite([FromBody] UserInviteDto inviteDto)
        {
            if (inviteDto.RoleID.HasValue)
            {
                var systemRole = Role.GetSingle(_dbContext, inviteDto.RoleID.Value);
                if (systemRole == null)
                {
                    return NotFound($"Could not find a Role with the ID {inviteDto.RoleID}");
                }
            }
            else
            {
                return BadRequest($"Role ID is required.");
            }

            var inviteModel = new KeystoneService.KeystoneInviteModel
            {
                FirstName = inviteDto.FirstName,
                LastName = inviteDto.LastName,
                Email = inviteDto.Email
            };

            var response = _keystoneService.Invite(inviteModel);
            if (response.StatusCode != HttpStatusCode.OK || response.Error != null)
            {
                ModelState.AddModelError("Email", $"There was a problem inviting the user to Keystone: {response.Error.Message}.");
                if (response.Error.ModelState != null)
                {
                    foreach (var modelStateKey in response.Error.ModelState.Keys)
                    {
                        foreach (var err in response.Error.ModelState[modelStateKey])
                        {
                            ModelState.AddModelError(modelStateKey, err);
                        }
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var keystoneUser = response.Payload.Claims;
            var existingUser = Rio.EFModels.Entities.User.GetSingle(_dbContext, keystoneUser.UserGuid);
            if (existingUser != null)
            {
                return Ok(existingUser);
            }

            var newUser = new UserUpsertDto
            {
                FirstName = keystoneUser.FirstName,
                LastName = keystoneUser.LastName,
                OrganizationName = keystoneUser.OrganizationName,
                Email = keystoneUser.Email,
                PhoneNumber = keystoneUser.PrimaryPhone,
                RoleID = inviteDto.RoleID.Value
            };

            var user = Rio.EFModels.Entities.User.CreateNewUser(_dbContext, newUser, keystoneUser.LoginName, keystoneUser.UserGuid);
            return Ok(user);
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<UserDto>> Get()
        {
            var userDtos = Rio.EFModels.Entities.User.GetList(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/{userID}")]
        public ActionResult<UserDto> GetFromUserID([FromRoute] int userID)
        {
            var userDto = Rio.EFModels.Entities.User.GetSingle(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }

        [HttpGet("user-claims/{globalID}")]
        public ActionResult<UserDto> GetFromGlobalID([FromRoute] string globalID)
        {
            var isValidGuid = Guid.TryParse(globalID, out var globalIDAsGuid);
            if (!isValidGuid)
            {
                return BadRequest();
            }

            var userDto = Rio.EFModels.Entities.User.GetSingle(_dbContext, globalIDAsGuid);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }

        [HttpPut("users/{userID}")]
        [RequiresValidJSONBodyFilter("Could not parse a valid User Upsert JSON object from the Request Body.")]
        public ActionResult<UserDto> UpdateUser([FromRoute] int userID, [FromBody] UserUpsertDto userEditDto)
        {
            var userDto = Rio.EFModels.Entities.User.GetSingle(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            var validationMessages = Rio.EFModels.Entities.User.ValidateUpdate(_dbContext, userEditDto, userDto.UserID);
            validationMessages.ForEach(vm => {
                ModelState.AddModelError(vm.Type, vm.Message);
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var systemRole = Role.GetSingle(_dbContext, userEditDto.RoleID.Value);
            if (systemRole == null)
            {
                return NotFound($"Could not find a System Role with the ID {userEditDto.RoleID}");
            }

            var updatedUserDto = Rio.EFModels.Entities.User.UpdateUserEntity(_dbContext, userID, userEditDto);
            return Ok(updatedUserDto);
        }
    }
}