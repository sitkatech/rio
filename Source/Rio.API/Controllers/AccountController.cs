﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.ParcelAllocation;
using Rio.Models.DataTransferObjects.WaterUsage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;
using Rio.Models.DataTransferObjects.WaterTransfer;

namespace Rio.API.Controllers
{
    [ApiController]
    public class AccountController : SitkaController<AccountController>
    {
        public AccountController(RioDbContext dbContext, ILogger<AccountController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("accountStatus")]
        [UserManageFeature]
        public IActionResult Get()
        {
            var accountStatusDtos = AccountStatus.List(_dbContext);
            return Ok(accountStatusDtos);
        }

        [HttpGet("/accounts")]
        [ManagerDashboardFeature]
        public ActionResult<List<AccountDto>> ListAllAccounts()
        {
            var accountDtos = Account.List(_dbContext);
            return accountDtos;
        }

        [HttpGet("/account/{accountID}")]
        [ManagerDashboardFeature]
        public ActionResult<AccountDto> GetAccountByID([FromRoute] int accountID)
        {
            var accountDto = Account.GetByAccountID(_dbContext, accountID);
            return RequireNotNullThrowNotFound(accountDto, "Account", accountID);
        }

        [HttpGet("/account/account-number/{accountNumber}")]
        [ParcelViewFeature]
        public ActionResult<AccountDto> GetAccountByAccountNumber([FromRoute] int accountNumber)
        {
            var accountDto = Account.GetByAccountNumber(_dbContext, accountNumber);
            if (ThrowNotFound(accountDto, "Account", accountNumber, out var actionResult))
            {
                return actionResult;
            }

            var userDto = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            if (userDto == null || userDto.Role.RoleID == (int) RoleEnum.LandOwner &&
                !Account.UserIDHasAccessToAccountID(_dbContext, userDto.UserID, accountDto.AccountID))
            {
                return Forbid();
            }

            return Ok(accountDto);
        }

        [HttpGet("/account/account-verification-key/{accountVerificationKey}")]
        public ActionResult<AccountDto> GetAccountByAccountVerificationKey([FromRoute] string accountVerificationKey)
        {
            var accountDto = Account.GetByAccountVerificationKey(_dbContext, accountVerificationKey);
            return RequireNotNullThrowNotFound(accountDto, "Account", accountVerificationKey);
        }

        [HttpPut("/account/{accountID}")]
        [UserManageFeature]
        public ActionResult<AccountDto> UpdateAccount([FromRoute] int accountID, [FromBody] AccountUpdateDto accountUpdateDto)
        {
            var accountDto = Account.GetByAccountID(_dbContext, accountID);
            if (ThrowNotFound(accountDto, "Account", accountID, out var actionResult))
            {
                return actionResult;
            }

            var accountStatus = AccountStatus.GetByAccountStatusID(_dbContext, accountUpdateDto.AccountStatusID);
            if (ThrowNotFound(accountStatus, "Account Status", accountUpdateDto.AccountStatusID, out var actionResult2))
            {
                return actionResult2;
            }

            var updatedUserDto = Account.UpdateAccountEntity(_dbContext, accountID, accountUpdateDto, _rioConfiguration.VerificationKeyChars);
            return Ok(updatedUserDto);
        }

        [HttpPost("/account/new")]
        [UserManageFeature]
        public ActionResult<AccountDto> CreateAccount([FromBody] AccountUpdateDto accountUpdateDto)
        {

            var accountStatus = AccountStatus.GetByAccountStatusID(_dbContext, accountUpdateDto.AccountStatusID);
            if (ThrowNotFound(accountStatus, "Account Status", accountUpdateDto.AccountStatusID, out var actionResult))
            {
                return actionResult;
            }

            var updatedUserDto = Account.CreateAccountEntity(_dbContext, accountUpdateDto, _rioConfiguration.VerificationKeyChars);
            return Ok(updatedUserDto);
        }

        [HttpPut("/account/{accountID}/edit-users")]
        [UserManageFeature]
        public ActionResult<AccountDto> EditUsers([FromRoute] int accountID, [FromBody] AccountEditUsersDto accountEditUsersDto)
        {
            var accountDto = Account.GetByAccountID(_dbContext, accountID);
            if (ThrowNotFound(accountDto, "Account", accountID, out var actionResult))
            {
                return actionResult;
            }

            if (!EFModels.Entities.User.ValidateAllExist(_dbContext, accountEditUsersDto.UserIDs))
            {
                return NotFound("One or more of the User IDs was invalid.");
            }

            if (EFModels.Entities.User.CheckIfUsersAreAdministrators(_dbContext, accountEditUsersDto.UserIDs))
            {
                return BadRequest(
                    ("One or more of the users in the list are Administrators. Administrators have access to all accounts by default, so please remove any Administrators from the list and try again."
                    ));
            }

            var updatedAccount = Account.SetAssociatedUsers(_dbContext, accountDto, accountEditUsersDto.UserIDs, out var addedUserIDs);

            var addedUsers = EFModels.Entities.User.GetByUserID(_dbContext, addedUserIDs);

            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateAddedUserEmails(_rioConfiguration.WEB_URL, updatedAccount, addedUsers);
            foreach (var mailMessage in mailMessages)
            {
                SendEmailMessage(smtpClient, mailMessage);
            }

            return Ok(updatedAccount);
        }

        [HttpGet("accounts/{accountID}/parcels/{year}")]
        [UserViewFeature]
        public ActionResult<List<ParcelDto>> ListParcelsByAccountID([FromRoute] int accountID, [FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByAccountIDAndYear(_dbContext, accountID, year);
            if (parcelDtos == null)
            {
                return NotFound();
            }

            return Ok(parcelDtos);
        }

        [HttpGet("accounts/{accountID}/postings")]
        [UserViewFeature]
        public ActionResult<List<PostingDto>> ListPostingsByAccountID([FromRoute] int accountID)
        {
            var postingDtos = Posting.ListByAccountID(_dbContext, accountID);
            if (postingDtos == null)
            {
                return NotFound();
            }

            return Ok(postingDtos);
        }

        [HttpGet("accounts/{accountID}/parcel-ledgers")]
        [UserViewFeature]
        public ActionResult<List<ParcelLedgerDto>> ListParcelLedgersByAccountID([FromRoute] int accountID)
        {
            var parcelLedgerDtos = ParcelLedger.ListByAccountID(_dbContext, accountID);
            if (parcelLedgerDtos == null)
            {
                return NotFound();
            }

            return Ok(parcelLedgerDtos);
        }

        [HttpGet("accounts/{accountID}/account-reconciliation-parcels")]
        [UserViewFeature]
        public ActionResult<ParcelSimpleDto> GetAccountReconciliationParcelsByAccountID([FromRoute] int accountID)
        {
            var parcelSimpleDtos = AccountReconciliation.ListParcelsByAccountID(_dbContext, accountID);
            return Ok(parcelSimpleDtos);
        }

        [HttpGet("accounts/water-usage-overview/{year}")]
        [ManagerDashboardFeature]
        public ActionResult<WaterUsageOverviewDto> GetWaterUsageOverview([FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByAccountIDsAndYear(_dbContext, _dbContext.Accounts.Select(x => x.AccountID).ToList(), year);
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var waterUsageOverviewDto = GetWaterUsageOverviewDtoForParcelIDs(parcelIDs);

            return Ok(waterUsageOverviewDto);
        }

        private WaterUsageOverviewDto GetWaterUsageOverviewDtoForParcelIDs(List<int> parcelIDs)
        {
            var parcelLedgerDtos = ParcelLedger.GetUsagesByParcelIDs(_dbContext, parcelIDs).Select(x => x.AsDto()).ToList();

            var cumulativeWaterUsageByYearDtos = parcelLedgerDtos.GroupBy(x => x.WaterYear)
                .Select(x =>
                new CumulativeWaterUsageByYearDto
                {
                    Year = x.Key, 
                    CumulativeWaterUsage = GetCurrentWaterUsageOverview(x)
                }).ToList();

            var historicWaterUsageOverview =
                GetHistoricWaterUsageOverview(cumulativeWaterUsageByYearDtos.SelectMany(x => x.CumulativeWaterUsage).ToList());

            // chart data can't have null values, so null cumulativewaterusage values are set to 0 after calculating historic usage (which relies on null values to know which months to exclude from calculation)
            foreach (var cumulativeWaterUsageByMonthDto in cumulativeWaterUsageByYearDtos
                .SelectMany(x => x.CumulativeWaterUsage).Where(y => y.CumulativeWaterUsageInAcreFeet == null))
            {
                cumulativeWaterUsageByMonthDto.CumulativeWaterUsageInAcreFeet = 0;
            }

            var waterUsageOverviewDto = new WaterUsageOverviewDto
            {
                Current = cumulativeWaterUsageByYearDtos, 
                Historic = historicWaterUsageOverview
            };
            return waterUsageOverviewDto;
        }

        private static List<CumulativeWaterUsageByMonthDto> GetHistoricWaterUsageOverview(List<CumulativeWaterUsageByMonthDto> waterUsageOverviewDtos)
        {
            var monthlyWaterUsageOverviewDtos = waterUsageOverviewDtos.GroupBy(x => x.Month)
                .Select(x => new CumulativeWaterUsageByMonthDto
                {
                    Month = x.Key, 
                    CumulativeWaterUsageInAcreFeet = Math.Round(x.Where(y => y.CumulativeWaterUsageInAcreFeet.HasValue)
                        .Average(y => y.CumulativeWaterUsageInAcreFeet.Value), 1)
                });

            return monthlyWaterUsageOverviewDtos.ToList();
        }

        private static List<CumulativeWaterUsageByMonthDto> GetCurrentWaterUsageOverview(IGrouping<int, ParcelLedgerDto> parcelLedgerDtos)
        {
            var parcelLedgersGroupedByMonth = parcelLedgerDtos.GroupBy(x => x.WaterMonth).ToList();
            var monthlyWaterUsageOverviewDtos = new List<CumulativeWaterUsageByMonthDto>();

            decimal cumulativeTotal = 0;

            for (var i = 1; i < 13; i++)
            {
                var grouping = parcelLedgersGroupedByMonth.SingleOrDefault(x => x.Key == i);
                cumulativeTotal += grouping?.Sum(x => x.TransactionAmount) ?? 0;
                var monthlyWaterUsageOverviewDto = new CumulativeWaterUsageByMonthDto()
                {
                    Month = ((DateUtilities.Month)i).ShortMonthName(),
                    CumulativeWaterUsageInAcreFeet = grouping == null ? null : Math.Abs(cumulativeTotal)
                };

                monthlyWaterUsageOverviewDtos.Add(monthlyWaterUsageOverviewDto);
            }

            return monthlyWaterUsageOverviewDtos;
        }

        private List<MailMessage> GenerateAddedUserEmails(string rioUrl, AccountDto addedAccount, IEnumerable<UserDto> addedUsers)
        {

            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The following account has been added to your profile in the {_rioConfiguration.PlatformLongName}. You can now manage this accounts in the Water Accounting Platform:<br/><br/>
{addedAccount.AccountDisplayName}<br/><br/>
You can view parcels associated with this account and the water allocation and usage of those parcels on your <a href='{rioUrl}/landowner-dashboard/{addedAccount.AccountNumber}'>Landowner Dashboard</a>";

            var mailTos = addedUsers;
            foreach(var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = $"{_rioConfiguration.PlatformLongName}: Water Accounts Added",
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
            mailMessage.From = smtpClient.GetDefaultEmailFrom();
            mailMessage.ReplyToList.Add(_rioConfiguration.LeadOrganizationEmail);
            SitkaSmtpClientService.AddBccRecipientsToEmail(mailMessage, EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
            smtpClient.Send(mailMessage);
        }

    }
}
