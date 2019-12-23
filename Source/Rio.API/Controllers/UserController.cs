using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System;
using System.Collections.Generic;
using System.Net;

namespace Rio.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
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
        [UserManageFeature]
        public IActionResult InviteUser([FromBody] UserInviteDto inviteDto)
        {
            if (inviteDto.RoleID.HasValue)
            {
                var role = Role.GetByRoleID(_dbContext, inviteDto.RoleID.Value);
                if (role == null)
                {
                    return NotFound($"Could not find a Role with the ID {inviteDto.RoleID}");
                }
            }
            else
            {
                return BadRequest("Role ID is required.");
            }

            const string applicationName = "Rosedale-Rio Bravo Water Trading Platform";
            const string rioBravoWaterStorageDistrict = "Rosedale-Rio Bravo Water Storage District";
            var inviteModel = new KeystoneService.KeystoneInviteModel
            {
                FirstName = inviteDto.FirstName,
                LastName = inviteDto.LastName,
                Email = inviteDto.Email,
                Subject = $"Invitation to the {applicationName}",
                WelcomeText = $"You are receiving this notification because you are identified as a participant in the pilot phase of the {applicationName}, an online platform provided by the {rioBravoWaterStorageDistrict}.",
                SiteName = applicationName,
                SignatureBlock = $"{rioBravoWaterStorageDistrict}<br /><a href='mailto:admin@rrbwsd.com'>admin@rrbwsd.com</a><br />(661) 589-6045<br /><a href='https://www.rrbwsd.com'>https://www.rrbwsd.com</a>",
                RedirectURL = "https://rrbwatertrading.sitkatech.com"
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
            var existingUser = EFModels.Entities.User.GetByEmail(_dbContext, inviteDto.Email);
            if (existingUser != null)
            {
                existingUser = EFModels.Entities.User.UpdateUserGuid(_dbContext, existingUser.UserID, keystoneUser.UserGuid);
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
        [UserManageFeature]
        public ActionResult<IEnumerable<UserDto>> List()
        {
            var userDtos = EFModels.Entities.User.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/{userID}")]
        [UserViewFeature]
        public ActionResult<UserDto> GetByUserID([FromRoute] int userID)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }

        [HttpGet("user-claims/{globalID}")]
        public ActionResult<UserDto> GetByGlobalID([FromRoute] string globalID)
        {
            var isValidGuid = Guid.TryParse(globalID, out var globalIDAsGuid);
            if (!isValidGuid)
            {
                return BadRequest();
            }

            var userDto = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, globalIDAsGuid);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }

        [HttpGet("user/{userID}/accounts")]
        [UserManageFeature]
        public ActionResult<List<AccountSimpleDto>> ListAccountsByUserID([FromRoute] int userID)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(Account.ListByUserID(_dbContext, userID));
        }

        [HttpPut("users/{userID}")]
        [UserManageFeature]
        public ActionResult<UserDto> UpdateUser([FromRoute] int userID, [FromBody] UserUpsertDto userUpsertDto)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            var validationMessages = Rio.EFModels.Entities.User.ValidateUpdate(_dbContext, userUpsertDto, userDto.UserID);
            validationMessages.ForEach(vm => {
                ModelState.AddModelError(vm.Type, vm.Message);
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = Role.GetByRoleID(_dbContext, userUpsertDto.RoleID.GetValueOrDefault());
            if (role == null)
            {
                return NotFound($"Could not find a System Role with the ID {userUpsertDto.RoleID}");
            }

            var updatedUserDto = Rio.EFModels.Entities.User.UpdateUserEntity(_dbContext, userID, userUpsertDto);
            return Ok(updatedUserDto);
        }

        [HttpGet("users/{userID}/postings")]
        [UserViewFeature]
        public ActionResult<List<PostingDto>> ListPostingsByUserID([FromRoute] int userID)
        {
            var postingDtos = Posting.ListByUserID(_dbContext, userID);
            if (postingDtos == null)
            {
                return NotFound();
            }

            return Ok(postingDtos);
        }

        [HttpGet("users/{userID}/water-transfers")]
        [UserViewFeature]
        public ActionResult<List<WaterTransferDto>> ListWaterTransfersByUserID([FromRoute] int userID)
        {
            var waterTransferDtos = WaterTransfer.ListByAccountID(_dbContext, userID);
            if (waterTransferDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferDtos);
        }


        [HttpGet("landowner-usage-report/{year}")]
        [UserManageFeature]
        public ActionResult<List<LandownerUsageReportDto>> GetLandOwnerUsageReport([FromRoute] int year)
        {
            var landownerUsageReportDtos = LandownerUsageReport.GetByYear(_dbContext, year);
            return Ok(landownerUsageReportDtos);
        }
    }
}