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
            var mailMessages = GenerateUserCreatedEmail(_rioConfiguration.RIO_WEB_URL, user, _dbContext);
            foreach (var mailMessage in mailMessages)
            {
                SendEmailMessage(smtpClient, mailMessage);
            }

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

            // todo: get added accounts; send email to user that these accounts have been added
            return Ok(EFModels.Entities.User.SetAssociatedAccounts(_dbContext, userDto, userEditAccountsDto.AccountIDs));
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

        private static List<MailMessage> GenerateAddedAccountsEmail(string rioUrl, OfferDto offer,
            TradeDto currentTrade, PostingDto posting, WaterTransferDto waterTransfer)
        {
            // TODO: Make this be what it's supposed to be instead of what it is
            AccountDto buyer;
            AccountDto seller;
            if (currentTrade.CreateAccount.AccountID == posting.CreateAccount.AccountID)
            {
                if (posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy)
                {
                    buyer = posting.CreateAccount;
                    seller = currentTrade.CreateAccount;
                }
                else
                {
                    buyer = currentTrade.CreateAccount;
                    seller = posting.CreateAccount;
                }
            }
            else
            {
                if (posting.PostingType.PostingTypeID == (int)PostingTypeEnum.OfferToBuy)
                {
                    buyer = posting.CreateAccount;
                    seller = currentTrade.CreateAccount;
                }
                else
                {
                    buyer = currentTrade.CreateAccount;
                    seller = posting.CreateAccount;
                }
            }

            var mailMessages = new List<MailMessage>();
            var messageBody = $@"Your offer to trade water has been accepted.
<ul>
    <li><strong>Buyer:</strong> {buyer.AccountName} ({string.Join(", ", buyer.Users.Select(x => x.Email))})</li>
    <li><strong>Seller:</strong> {seller.AccountName} ({string.Join(", ", seller.Users.Select(x => x.Email))})</li>
    <li><strong>Quantity:</strong> {offer.Quantity} acre-feet</li>
    <li><strong>Unit Price:</strong> {offer.Price:$#,##0.00} per acre-foot</li>
    <li><strong>Total Price:</strong> {(offer.Price * offer.Quantity):$#,##0.00}</li>
</ul>
To finalize this transaction, the buyer and seller must complete payment and any other terms of the transaction. Once payment is complete, the trade must be confirmed by both parties within the Water Accounting Platform before the district will recognize the transfer.
<br /><br />
<a href=""{rioUrl}/register-transfer/{waterTransfer.WaterTransferID}"">Confirm Transfer</a>
{SitkaSmtpClientService.GetDefaultEmailSignature()}";
            var mailTos = (new List<AccountDto> { buyer, seller }).SelectMany(x => x.Users);
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = $"Trade {currentTrade.TradeNumber} Accepted",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }

        private static List<MailMessage> GenerateUserCreatedEmail(string rioUrl, UserDto user, RioDbContext dbContext)
        {
            var mailMessages = new List<MailMessage>();
            var messageBody = $@"A new user has signed up to the Rosedale-Rio Bravo Water Accounting Platform: <br/><br/>
 {user.FullName} ({user.Email}) <br/><br/>
As an administrator of the Water Accounting Platform, you can assign them a role and associate them with a Billing Account by following <a href='{rioUrl}/users/{user.UserID}'>this link</a>. <br/><br/>
{SitkaSmtpClientService.GetDefaultEmailSignature()}";

            // todo!
             var administrators = EFModels.Entities.User.ListByRole(dbContext, RoleEnum.Admin);

             var mailTos = administrators;
            foreach (var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = $"New User in Rosedale-Rio Bravo Water Accounting Platform",
                    Body = $"Hello {mailTo.FullName},<br /><br />{messageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = SitkaSmtpClientService.GetDefaultEmailFrom();
            SitkaSmtpClientService.AddReplyToEmail(mailMessage);
            SitkaSmtpClientService.AddAdminsAsBccRecipientsToEmail(mailMessage, EFModels.Entities.User.ListByRole(_dbContext, RoleEnum.Admin));
            smtpClient.Send(mailMessage);
        }
    }
}
