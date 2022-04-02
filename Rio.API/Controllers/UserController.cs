using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Rio.API.Controllers
{
    [ApiController]
    public class UserController : SitkaController<UserController>
    {
        public UserController(RioDbContext dbContext, ILogger<UserController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpPost("/users/invite")]
        [UserManageFeature]
        public async Task<IActionResult> InviteUser([FromBody] UserInviteDto inviteDto)
        {
            if (inviteDto.RoleID.HasValue)
            {
                var role = Role.AllLookupDictionary[inviteDto.RoleID.Value];
                if (role == null)
                {
                    return NotFound($"Could not find a Role with the ID {inviteDto.RoleID}");
                }
            }
            else
            {
                return BadRequest("Role ID is required.");
            }

            var welcomeText =
                $"You are receiving this notification because an administrator of the {_rioConfiguration.PlatformLongName}, an online service of the {_rioConfiguration.LeadOrganizationLongName}, has invited you to create an account.";
            var response = await KeystoneInviteUserApiResponse(inviteDto.FirstName, inviteDto.LastName, inviteDto.Email, welcomeText);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = GetAndUpdateUserFromEmailOrReturnNewUser(response, inviteDto.Email, (int) inviteDto.RoleID);
            return Ok(user);
        }

        [HttpPost("/users/invite-partner")]
        [UserViewFeature]
        public async Task<IActionResult> InvitePartner([FromBody] UserPartnerInviteDto inviteDto)
        {
            var userFromContextDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            if (userFromContextDto == null)
            {
                return NotFound($"Could not find Current User");
            }

            if (!Account.ValidateAllExist(_dbContext, inviteDto.AccountIDs))
            {
                return NotFound("One or more of the Account IDs was invalid.");
            }

            var welcomeText =
                $"You are receiving this notification because {userFromContextDto.FullName} has invited you to create a user profile for the {_rioConfiguration.PlatformLongName}, an online service of the {_rioConfiguration.LeadOrganizationLongName}, and granted you access to view their Water Account Dashboard.";
            var response = await KeystoneInviteUserApiResponse(inviteDto.FirstName, inviteDto.LastName, inviteDto.Email, welcomeText);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = GetAndUpdateUserFromEmailOrReturnNewUser(response, inviteDto.Email, (int) RoleEnum.LandOwner);

            //Because there's a chance the user already exists, just grab accounts for them and merge with the ids that came in
            var currentAccountIDsForUser = Account.ListByUserID(_dbContext, user.UserID).Select(x => x.AccountID).ToList();
            var allAccountIDsForUser = inviteDto.AccountIDs.Union(currentAccountIDsForUser).ToList();

            EFModels.Entities.User.SetAssociatedAccounts(_dbContext, user.UserID, allAccountIDsForUser);
            return Ok(user);
        }

        private UserDto GetAndUpdateUserFromEmailOrReturnNewUser(KeystoneService.KeystoneApiResponse<KeystoneService.KeystoneNewUserModel> response, string email, int roleID)
        {
            var keystoneUser = response.Payload.Claims;
            var existingUser = EFModels.Entities.User.GetByEmail(_dbContext, email);
            if (existingUser != null)
            {
                existingUser = EFModels.Entities.User.UpdateUserGuid(_dbContext, existingUser.UserID, keystoneUser.UserGuid);
                return existingUser;
            }

            var newUser = new UserUpsertDto
            {
                FirstName = keystoneUser.FirstName,
                LastName = keystoneUser.LastName,
                OrganizationName = keystoneUser.OrganizationName,
                Email = keystoneUser.Email,
                PhoneNumber = keystoneUser.PrimaryPhone,
                RoleID = roleID
            };

            var user = EFModels.Entities.User.CreateNewUser(_dbContext, newUser, keystoneUser.LoginName,
                keystoneUser.UserGuid);
            return user;
        }

        private async Task<KeystoneService.KeystoneApiResponse<KeystoneService.KeystoneNewUserModel>> KeystoneInviteUserApiResponse(string firstName, string lastName, string email, string welcomeText)
        {
            var applicationName = $"{_rioConfiguration.PlatformLongName}";
            var rioBravoWaterStorageDistrict = $"{_rioConfiguration.LeadOrganizationLongName}";
            var inviteModel = new KeystoneService.KeystoneInviteModel
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Subject = $"Invitation to the {applicationName}",
                WelcomeText = welcomeText,
                SiteName = applicationName,
                SignatureBlock =
                    $"{rioBravoWaterStorageDistrict}<br /><a href='mailto:{_rioConfiguration.LeadOrganizationEmail}'>{_rioConfiguration.LeadOrganizationEmail}</a><br/><a href='{_rioConfiguration.LeadOrganizationHomeUrl}'>{_rioConfiguration.LeadOrganizationHomeUrl}</a>",
                RedirectURL = _rioConfiguration.KEYSTONE_REDIRECT_URL
            };

            var response = await _keystoneService.Invite(inviteModel);
            if (response.StatusCode != HttpStatusCode.OK || response.Error != null)
            {
                ModelState.AddModelError("Email",
                    $"There was a problem inviting the user to Keystone: {response.Error.Message}.");
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

            return response;
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
                {Count = _dbContext.Users.Count(x => x.RoleID == (int) RoleEnum.Unassigned)};
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

        [HttpGet("user/{userID}/accounts-include-parcels")]
        [UserViewFeature]
        public ActionResult<List<AccountIncludeParcelsDto>> ListAccountsByUserIDIncludeParcels([FromRoute] int userID)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            if (userDto.Role.RoleID == (int)RoleEnum.Admin)
            {
                return Ok(Account.ListIncludeParcels(_dbContext));
            }

            return Ok(Account.ListByUserIDIncludeParcels(_dbContext, userID));
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

            var role = Role.AllLookupDictionary[userUpsertDto.RoleID.GetValueOrDefault()];
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

        [HttpPut("/user/add-accounts-via-verification-key")]
        [UserViewFeature]
        public ActionResult<UserDto> AddAccountsForCurrentUserUsingAccountVerificationKeys([FromBody] UserEditAccountsDto userEditAccountsDto)
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

            EFModels.Entities.User.SetAssociatedAccounts(_dbContext, userFromContextDto.UserID, allAccountIDsForUser);

            if (userFromContextDto.Role.RoleID == (int) RoleEnum.Unassigned)
            {
                var userUpsertDto = new UserUpsertDto
                {
                    Email = userFromContextDto.Email,
                    FirstName = userFromContextDto.FirstName,
                    LastName = userFromContextDto.LastName,
                    RoleID = (int)RoleEnum.LandOwner,
                    PhoneNumber = userFromContextDto.Phone
                };

                userFromContextDto = EFModels.Entities.User.UpdateUserEntity(_dbContext, userFromContextDto.UserID, userUpsertDto);
            }

            Account.UpdateAccountVerificationKeyLastUsedDateForAccountIDs(_dbContext, userEditAccountsDto.AccountIDs);

            return Ok(userFromContextDto);
        }

        [HttpDelete("/user/remove-account/{accountID}")]
        [UserViewFeature]
        public ActionResult RemoveAccountByIDForCurrentUser([FromRoute] int accountID)
        {
            var userFromContextDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            if (userFromContextDto == null)
            {
                return NotFound($"Could not find Current User");
            }

            if (Account.GetByAccountID(_dbContext, accountID) == null)
            {
                return NotFound($"The Account with AccountID:{accountID} could not be found.");
            }

            EFModels.Entities.User.RemoveAssociatedAccount(_dbContext, userFromContextDto.UserID, accountID);

            return Ok();
        }

        [HttpPut("/users/{userID}/edit-accounts")]
        [UserManageFeature]
        public ActionResult<UserDto> EditAccounts([FromRoute] int userID, [FromBody] UserEditAccountsDto userEditAccountsDto)
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

            var addedAccountIDs = EFModels.Entities.User.SetAssociatedAccounts(_dbContext, userID, userEditAccountsDto.AccountIDs);
            var addedAccounts = Account.GetByAccountID(_dbContext, addedAccountIDs);

            if (addedAccounts != null && addedAccounts.Count > 0)
            {
                SendEmailToLandownerAndAdmins(userDto, addedAccounts);
            }

            return Ok(userDto);
        }

        private void SendEmailToLandownerAndAdmins(UserDto userDto, List<AccountDto> addedAccounts)
        {
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateAddedAccountsEmail(_rioConfiguration.WEB_URL, userDto, addedAccounts);
            foreach (var mailMessage in mailMessages)
            {
                SitkaSmtpClientService.AddBccRecipientsToEmail(mailMessage,
                    EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
                SendEmailMessage(smtpClient, mailMessage);
            }
        }

        [HttpGet("users/{userID}/parcels/{year}")]
        [UserViewFeature]
        public ActionResult<List<ParcelSimpleDto>> ListParcelsByAccountID([FromRoute] int userID, [FromRoute] int year)
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
            var landownerUsageReportDtos = LandownerUsageReport.GetByYear(_dbContext, year).ToList();

            var landownerWaterSupplyBreakdownForYear = ParcelLedgers.GetLandownerWaterSupplyBreakdownForYear(_dbContext, year);

            foreach (var landownerUsageReportDto in landownerUsageReportDtos)
            {
                var accountWaterSupplyBreakdown = landownerWaterSupplyBreakdownForYear
                    .SingleOrDefault(x => x.AccountID == landownerUsageReportDto.AccountID);
                landownerUsageReportDto.WaterSupplyByWaterType = accountWaterSupplyBreakdown?.WaterSupplyByWaterType;
            } 

            return Ok(landownerUsageReportDtos);
        }

        private List<MailMessage> GenerateAddedAccountsEmail(string rioUrl, UserDto updatedUser, IEnumerable<AccountDto> addedAccounts)
        {
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The following accounts have been added to your profile in the {_rioConfiguration.PlatformLongName}. You can now manage these accounts in the {_rioConfiguration.PlatformShortName}:<br/><br/>";
            foreach (var account in addedAccounts)
            {
                messageBody += $"{account.AccountDisplayName} <br/><br/>";
            }

            messageBody += $"You can view parcels associated with these accounts and the water supply and usage of those parcels by going to your <a href='{rioUrl}/water-accounts'>Water Accounts List</a> and navigating to the appropriate account.";

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
