﻿using Microsoft.AspNetCore.Mvc;
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

            string applicationName = $"{_rioConfiguration.PlatformLongName}";
            string rioBravoWaterStorageDistrict = $"{_rioConfiguration.LeadOrganizationLongName}";
            var inviteModel = new KeystoneService.KeystoneInviteModel
            {
                FirstName = inviteDto.FirstName,
                LastName = inviteDto.LastName,
                Email = inviteDto.Email,
                Subject = $"Invitation to the {applicationName}",
                WelcomeText = $"You are receiving this notification because an administrator of the {applicationName}, an online service of the {rioBravoWaterStorageDistrict}, has invited you to create an account.",
                SiteName = applicationName,
                SignatureBlock = $"{rioBravoWaterStorageDistrict}<br /><a href='mailto:{_rioConfiguration.LeadOrganizationEmail}'>{_rioConfiguration.LeadOrganizationEmail}</a><a href='{_rioConfiguration.LeadOrganizationHomeUrl}'>{_rioConfiguration.LeadOrganizationHomeUrl}</a>",
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
            var mailMessage = GenerateUserCreatedEmail(_rioConfiguration.WEB_URL, user, _dbContext, smtpClient);
            SitkaSmtpClientService.AddCcRecipientsToEmail(mailMessage,
                        EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
            SendEmailMessage(smtpClient, mailMessage);

            return Ok(user);
        }

        [HttpGet("users")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<UserDetailedDto>> List()
        {
            var userDtos = EFModels.Entities.User.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/landowners-and-demo-users")]
        [ManagerDashboardFeature]
        public ActionResult<IEnumerable<UserDetailedDto>> ListLandownerAndDemoUsers()
        {
            var userDtos = EFModels.Entities.User.ListByRole(_dbContext,
                new List<int>() {(int) RoleEnum.LandOwner, (int) RoleEnum.DemoUser});
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

        [HttpPut("/user/add-accounts")]
        [UserViewFeature]
        public ActionResult<UserDto> AddAccountsForCurrentUser([FromBody] UserEditAcountsDto userEditAccountsDto)
        {
            var userFromContextDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            if (userFromContextDto == null)
            {
                return NotFound($"Could not find Current User");
            }

            if (!Account.ValidateAllExist(_dbContext, userEditAccountsDto.AccountIDs))
            {
               return NotFound("One or more of the Account IDs was invalid.");
            }

            var currentAccountIDsForUser = Account.ListByUserID(_dbContext, userFromContextDto.UserID).Select(x => x.AccountID).ToList();
            var allAccountIDsForUser = userEditAccountsDto.AccountIDs.Union(currentAccountIDsForUser).ToList();

            var updatedUserDto = EFModels.Entities.User.SetAssociatedAccounts(_dbContext, userFromContextDto.UserID, allAccountIDsForUser, out var addedAccountIDs);

            if (updatedUserDto.Role.RoleID == (int) RoleEnum.Unassigned)
            {
                var userUpsertDto = new UserUpsertDto
                {
                    Email = updatedUserDto.Email,
                    FirstName = updatedUserDto.FirstName,
                    LastName = updatedUserDto.LastName,
                    RoleID = (int)RoleEnum.LandOwner,
                    PhoneNumber = updatedUserDto.Phone
                };

                updatedUserDto = EFModels.Entities.User.UpdateUserEntity(_dbContext, updatedUserDto.UserID, userUpsertDto);
            }

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

            var updatedUserDto = EFModels.Entities.User.SetAssociatedAccounts(_dbContext, userID, userEditAccountsDto.AccountIDs, out var addedAccountIDs);
            var addedAccounts = Account.GetByAccountID(_dbContext, addedAccountIDs);

            if (addedAccounts != null && addedAccounts.Count > 0)
            {
                SendEmailToLandownerAndAdmins(updatedUserDto, addedAccounts);
            }

            return Ok(updatedUserDto);
        }

        private void SendEmailToLandownerAndAdmins(UserDto updatedUserDto, List<AccountDto> addedAccounts)
        {
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateAddedAccountsEmail(_rioConfiguration.WEB_URL, updatedUserDto, addedAccounts);
            foreach (var mailMessage in mailMessages)
            {
                SitkaSmtpClientService.AddBccRecipientsToEmail(mailMessage,
                    EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
                SendEmailMessage(smtpClient, mailMessage);
            }
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
        [ManagerDashboardFeature]
        public ActionResult<List<LandownerUsageReportDto>> GetLandOwnerUsageReport([FromRoute] int year)
        {
            var landownerUsageReportDtos = LandownerUsageReport.GetByYear(_dbContext, year);

            var landownerAllocationBreakdownForYear = ParcelAllocation.GetLandownerAllocationBreakdownForYear(_dbContext, year);

            var landownerUsageReportDtosWithAllocation = landownerUsageReportDtos.Join(
                landownerAllocationBreakdownForYear, x => x.AccountID, y => y.AccountID,
                (x, y) =>
                {
                    x.Allocations = y.Allocations;
                    return x;
                });


            return Ok(landownerUsageReportDtosWithAllocation);
        }

        private List<MailMessage> GenerateAddedAccountsEmail(string rioUrl, UserDto updatedUser, IEnumerable<AccountDto> addedAccounts)
        {
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The following accounts have been added to your profile in the {_rioConfiguration.PlatformLongName}. You can now manage these accounts in the {_rioConfiguration.PlatformShortName}:<br/><br/>";
            foreach (var account in addedAccounts)
            {
                messageBody += $"{account.AccountDisplayName} <br/><br/>";
            }

            messageBody += $"You can view parcels associated with these accounts and the water allocation and usage of those parcels on your <a href='{rioUrl}/landowner-dashboard'>Landowner Dashboard</a>";

            var mailTo = updatedUser;
            var mailMessage = new MailMessage
            {
                Subject = $"{_rioConfiguration.PlatformLongName}: Water Accounts Added",
                Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
            };
            mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
            mailMessages.Add(mailMessage);
            return mailMessages;
        }

        private MailMessage GenerateUserCreatedEmail(string rioUrl, UserDto user, RioDbContext dbContext,
            SitkaSmtpClientService smtpClient)
        {
            var messageBody = $@"A new user has signed up to the {_rioConfiguration.PlatformLongName}: <br/><br/>
 {user.FullName} ({user.Email}) <br/><br/>
As an administrator of the {_rioConfiguration.PlatformShortName}, you can assign them a role and associate them with a Billing Account by following <a href='{rioUrl}/users/{user.UserID}'>this link</a>. <br/><br/>
{smtpClient.GetSupportNotificationEmailSignature()}";

            var mailMessage = new MailMessage
            {
                Subject = $"New User in {_rioConfiguration.PlatformLongName}",
                Body = $"Hello,<br /><br />{messageBody}",
            };

            mailMessage.To.Add(smtpClient.GetDefaultEmailFrom());
            return mailMessage;
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = smtpClient.GetDefaultEmailFrom();
            mailMessage.ReplyToList.Add(_rioConfiguration.LeadOrganizationEmail);
            smtpClient.Send(mailMessage);
        }
    }
}
