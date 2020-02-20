using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Rio.Models.DataTransferObjects.Offer;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<UserController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;

        public UserController(RioDbContext dbContext, ILogger<UserController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfigurationOptions)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfigurationOptions.Value;
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

            const string applicationName = "Rosedale-Rio Bravo Water Accounting Platform";
            const string rioBravoWaterStorageDistrict = "Rosedale-Rio Bravo Water Storage District";
            var inviteModel = new KeystoneService.KeystoneInviteModel
            {
                FirstName = inviteDto.FirstName,
                LastName = inviteDto.LastName,
                Email = inviteDto.Email,
                Subject = $"Invitation to the {applicationName}",
                WelcomeText = $"You are receiving this notification because an administrator of the {applicationName}, an online service of {rioBravoWaterStorageDistrict}, has invited you to create an account.",
                SiteName = applicationName,
                SignatureBlock = $"{rioBravoWaterStorageDistrict}<br /><a href='mailto:admin@rrbwsd.com'>admin@rrbwsd.com</a><br />(661) 589-6045<br /><a href='https://www.rrbwsd.com'>https://www.rrbwsd.com</a>",
                RedirectURL = _rioConfiguration.KEYSTONE_REDIRECT_URL
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

            var user = EFModels.Entities.User.CreateNewUser(_dbContext, newUser, keystoneUser.LoginName,
                keystoneUser.UserGuid);
            return Ok(user);
        }

        [HttpPost("users")]
        [LoggedInUnclassifiedFeature]
        public ActionResult<UserDto> CreateUser([FromBody] UserCreateDto userUpsertDto)
        {
            var user = EFModels.Entities.User.CreateNewUser(_dbContext, userUpsertDto, userUpsertDto.LoginName,
                userUpsertDto.UserGuid);

            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessage = GenerateUserCreatedEmail(_rioConfiguration.RIO_WEB_URL, user, _dbContext);
            SendEmailMessage(smtpClient, mailMessage, "cc");

            return Ok(user);
        }

        [HttpGet("users")]
        [UserManageFeature]
        public ActionResult<IEnumerable<UserDetailedDto>> List()
        {
            var userDtos = EFModels.Entities.User.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/unassigned-report")]
        [UserManageFeature]
        public ActionResult<UnassignedUserReportDto> GetUnassignedUserReport()
        {
            var report = new UnassignedUserReportDto
                {Count = _dbContext.User.Count(x => x.RoleID == (int) RoleEnum.Unassigned)};
            return Ok(report);
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
        [UserViewFeature]
        public ActionResult<List<AccountSimpleDto>> ListAccountsByUserID([FromRoute] int userID)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            if (userDto.Role.RoleID == (int) RoleEnum.Admin)
            {
                return Ok(Account.List(_dbContext));
            }
            else
            {
                return Ok(Account.ListByUserID(_dbContext, userID));
            }
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

        [HttpPut("users/set-disclaimer-acknowledged-date")]
        public ActionResult<UserDto> SetDisclaimerAcknowledgedDate([FromBody] int userID)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            var updatedUserDto = Rio.EFModels.Entities.User.SetDisclaimerAcknowledgedDate(_dbContext, userID);
            return Ok(updatedUserDto);
        }

        [HttpPut("/users/{userID}/edit-accounts")]
        [UserManageFeature]
        public ActionResult<UserDto> EditAccounts([FromRoute] int userID, [FromBody] UserEditAcountsDto userEditAccountsDto)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);

            if (userDto == null)
            {
                return NotFound($"Could not find an User with the ID {userID}.");
            }

            if (!Account.ValidateAllExist(_dbContext, userEditAccountsDto.AccountIDs))
            {
                return NotFound("One or more of the Account IDs was invalid.");
            }

            var updatedUserDto = EFModels.Entities.User.SetAssociatedAccounts(_dbContext, userDto, userEditAccountsDto.AccountIDs, out var addedAccountIDs);
            var addedAccounts = Account.GetByAccountID(_dbContext, addedAccountIDs);

            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateAddedAccountsEmail(_rioConfiguration.RIO_WEB_URL, updatedUserDto, addedAccounts);
            foreach (var mailMessage in mailMessages)
            {
                SendEmailMessage(smtpClient, mailMessage, "bcc");
            }

            return Ok(updatedUserDto);
        }

        [HttpGet("users/{userID}/parcels/{year}")]
        [UserViewFeature]
        public ActionResult<List<ParcelDto>> ListParcelsByAccountID([FromRoute] int userID, [FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByUserID(_dbContext, userID, year);
            if (parcelDtos == null)
            {
                return NotFound();
            }

            return Ok(parcelDtos);
        }

        [HttpGet("landowner-usage-report/{year}")]
        [UserManageFeature]
        public ActionResult<List<LandownerUsageReportDto>> GetLandOwnerUsageReport([FromRoute] int year)
        {
            var landownerUsageReportDtos = LandownerUsageReport.GetByYear(_dbContext, year);
            return Ok(landownerUsageReportDtos);
        }

        private static List<MailMessage> GenerateAddedAccountsEmail(string rioUrl, UserDto updatedUser, IEnumerable<AccountDto> addedAccounts)
        {
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The following accounts have been added to your profile in the Rosedale-Rio Bravo Water Accounting Platform. You can now manage these accounts in the Water Accounting Platform:<br/><br/>";
            foreach (var account in addedAccounts)
            {
                messageBody += $"{account.AccountDisplayName} <br/><br/>";
            }

            messageBody += $"You can view parcels associated with these accounts and the water allocation and usage of those parcels on your <a href='{rioUrl}/landowner-dashboard'>Landowner Dashboard</a>";

            var mailTo = updatedUser;
            var mailMessage = new MailMessage
            {
                Subject = $"Rosedale-Rio Bravo Water Accounting Platform: Water Accounts Added",
                Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
            };
            mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
            mailMessages.Add(mailMessage);
            return mailMessages;
        }

        private static MailMessage GenerateUserCreatedEmail(string rioUrl, UserDto user, RioDbContext dbContext)
        {
            var messageBody = $@"A new user has signed up to the Rosedale-Rio Bravo Water Accounting Platform: <br/><br/>
 {user.FullName} ({user.Email}) <br/><br/>
As an administrator of the Water Accounting Platform, you can assign them a role and associate them with a Billing Account by following <a href='{rioUrl}/users/{user.UserID}'>this link</a>. <br/><br/>
{SitkaSmtpClientService.GetSupportNotificationEmailSignature()}";

            var mailMessage = new MailMessage
            {
                Subject = $"New User in Rosedale-Rio Bravo Water Accounting Platform",
                Body = $"Hello,<br /><br />{messageBody}",
            };

            mailMessage.To.Add(SitkaSmtpClientService.GetDefaultEmailFrom());
            return mailMessage;
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage, string typeOfCCAdminsToReceive = "None")
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = SitkaSmtpClientService.GetDefaultEmailFrom();
            SitkaSmtpClientService.AddReplyToEmail(mailMessage);
            if (typeOfCCAdminsToReceive.ToLower() == "bcc")
            {
                SitkaSmtpClientService.AddAdminsAsBccRecipientsToEmail(mailMessage,
                    EFModels.Entities.User.AdminsThatReceiveSupportEmails(_dbContext));
            }
            else if (typeOfCCAdminsToReceive.ToLower() == "cc")
            {
                SitkaSmtpClientService.AddAdminsAsCCRecipientsToEmail(mailMessage,
                    EFModels.Entities.User.AdminsThatReceiveSupportEmails(_dbContext));
            }

            smtpClient.Send(mailMessage);
        }
    }
}
