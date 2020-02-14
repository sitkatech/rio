using Microsoft.AspNetCore.Mvc;
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
    public class AccountController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;

        public AccountController(RioDbContext dbContext, ILogger<AccountController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpGet("accountStatus")]
        [UserManageFeature]
        public IActionResult Get()
        {
            var accountStatusDtos = AccountStatus.List(_dbContext);
            return Ok(accountStatusDtos);
        }

        [HttpGet("/accounts")]
        [UserManageFeature]
        public ActionResult<List<AccountDto>> ListAllAccounts()
        {
            var accountDtos = Account.List(_dbContext);
            return accountDtos;
        }

        [HttpGet("/account/{accountID}")]
        [UserManageFeature]
        public ActionResult<AccountDto> GetAccountByID([FromRoute] int accountID)
        {
            var accountDto = Account.GetByAccountID(_dbContext, accountID);
            if (accountDto == null)
            {
                return NotFound();
            }

            return Ok(accountDto);
        }

        [HttpPut("/account/{accountID}")]
        [UserManageFeature]
        public ActionResult<AccountDto> UpdateAccount([FromRoute] int accountID, [FromBody] AccountUpdateDto accountUpdateDto)
        {
            var accountDto = Account.GetByAccountID(_dbContext, accountID);
            if (accountDto == null)
            {
                return NotFound(); 
            }

            var accountStatus = AccountStatus.GetByAccountStatusID(_dbContext, accountUpdateDto.AccountStatusID);
            if (accountStatus == null)
            {
                return NotFound($"Could not find a System AccountStatus with the ID {accountUpdateDto.AccountStatusID}");
            }

            var updatedUserDto = Account.UpdateAccountEntity(_dbContext, accountID, accountUpdateDto);
            return Ok(updatedUserDto);
        }

        [HttpPost("/account/new")]
        [UserManageFeature]
        public ActionResult<AccountDto> CreateAccount([FromBody] AccountUpdateDto accountUpdateDto)
        {

            var accountStatus = AccountStatus.GetByAccountStatusID(_dbContext, accountUpdateDto.AccountStatusID);
            if (accountStatus == null)
            {
                return NotFound($"Could not find an Account Status with the ID {accountUpdateDto.AccountStatusID}");
            }

            var updatedUserDto = Account.CreateAccountEntity(_dbContext, accountUpdateDto);
            return Ok(updatedUserDto);
        }

        [HttpPut("/account/{accountID}/edit-users")]
        [UserManageFeature]
        public ActionResult<AccountDto> EditUsers([FromRoute] int accountID, [FromBody] AccountEditUsersDto accountEditUsersDto)
        {
            var accountDto = Account.GetByAccountID(_dbContext, accountID);

            if (accountDto == null)
            {
                return NotFound($"Could not find an Account with the ID {accountID}.");
            }

            if (!EFModels.Entities.User.ValidateAllExist(_dbContext, accountEditUsersDto.UserIDs))
            {
                return NotFound("One or more of the User IDs was invalid.");
            }

            // todo: get added users, send them each an email that they were added to this account
            var updatedAccount = Account.SetAssociatedUsers(_dbContext, accountDto, accountEditUsersDto.UserIDs, out var addedUserIDs);

            var addedUsers = EFModels.Entities.User.GetByUserID(_dbContext, addedUserIDs);

            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateAddedUserEmails(_rioConfiguration.RIO_WEB_URL, updatedAccount, addedUsers);
            foreach (var mailMessage in mailMessages)
            {
                SendEmailMessage(smtpClient, mailMessage);
            }

            return Ok(updatedAccount);
        }


        // todo: goes to account controller and gets a new route
        [HttpGet("accounts/{accountID}/getParcelsAllocations/{year}")]
        [UserViewFeature]
        public ActionResult<List<ParcelAllocationDto>> ListParcelsAllocationByAccountID([FromRoute] int accountID, [FromRoute] int year)
        {
            var parcelDtosEnumerable = Parcel.ListByAccountID(_dbContext, accountID, year);
            if (parcelDtosEnumerable == null)
            {
                return NotFound();
            }

            var parcelDtos = parcelDtosEnumerable.ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();
            var parcelAllocationDtos = ParcelAllocation.ListByParcelID(_dbContext, parcelIDs);
            return Ok(parcelAllocationDtos);
        }

        [HttpGet("accounts/{accountID}/parcels/{year}")]
        [UserViewFeature]
        public ActionResult<List<ParcelDto>> ListParcelsByAccountID([FromRoute] int accountID, [FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByAccountID(_dbContext, accountID, year);
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

        [HttpGet("accounts/{accountID}/water-transfers")]
        [UserViewFeature]
        public ActionResult<List<WaterTransferDto>> ListWaterTransfersByUserID([FromRoute] int accountID)
        {
            var waterTransferDtos = WaterTransfer.ListByAccountID(_dbContext, accountID);
            if (waterTransferDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferDtos);
        }

        [HttpGet("accounts/{accountID}/parcel-water-usage/{year}")]
        [UserViewFeature]
        public ActionResult<List<ParcelMonthlyEvapotranspirationDto>> ListWaterUsagesByParcelAndAccountID([FromRoute] int accountID, [FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByAccountID(_dbContext, accountID, year).ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();
            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelIDs).Where(x => x.WaterYear == year);
            return Ok(parcelMonthlyEvapotranspirationDtos);
        }

        [HttpPut("accounts/{accountID}/{year}/saveParcelMonthlyEvapotranspirationOverrideValues")]
        [UserManageFeature]
        public ActionResult<List<ParcelMonthlyEvapotranspirationDto>> SaveParcelMonthlyEvapotranspirationOverrideValues( [FromRoute] int accountID, [FromRoute] int year,
            [FromBody] List<ParcelMonthlyEvapotranspirationDto> overriddenValues)
        {
            ParcelMonthlyEvapotranspiration.SaveParcelMonthlyUsageOverrides(_dbContext, accountID, year, overriddenValues);
            return Ok();
        }

        [HttpGet("accounts/{accountID}/water-usage/{year}")]
        [UserViewFeature]
        public ActionResult<List<WaterUsageByParcelDto>> ListWaterUsagesByAccountID([FromRoute] int accountID, [FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByAccountID(_dbContext, accountID, year).ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelIDs);

            var waterUsageDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterYear).Select(x =>
                new WaterUsageByParcelDto { Year = x.Key, WaterUsage = GetMonthlyWaterUsageDtos(x, parcelDtos) });

            return Ok(waterUsageDtos);
        }

        [HttpGet("accounts/{accountID}/water-usage-overview/{year}")]
        [UserViewFeature]
        public ActionResult<WaterUsageOverviewDto> GetWaterUsageOverviewByAccountID([FromRoute] int accountID, [FromRoute] int year)
        {
            var parcelDtos = Parcel.ListByAccountID(_dbContext, accountID, year);
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelIDs).ToList();

            var cumulativeWaterUsageByYearDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterYear).Select(x => new CumulativeWaterUsageByYearDto
                { Year = x.Key, CumulativeWaterUsage = GetCurrentWaterUsageOverview(x) }).ToList();

            var historicWaterUsageOverview = GetHistoricWaterUsageOverview(cumulativeWaterUsageByYearDtos.SelectMany(x => x.CumulativeWaterUsage).ToList());

            // the chart needs value to be non null, so we need to set the cumulativewaterusage values to be 0 for the null ones; we need them to be null originally when calculating historic since we don't want them to count
            foreach (var cumulativeWaterUsageByMonthDto in cumulativeWaterUsageByYearDtos.SelectMany(x => x.CumulativeWaterUsage).Where(y => y.CumulativeWaterUsageInAcreFeet == null))
            {
                cumulativeWaterUsageByMonthDto.CumulativeWaterUsageInAcreFeet = 0;
            }

            var waterUsageOverviewDto = new WaterUsageOverviewDto { Current = cumulativeWaterUsageByYearDtos, Historic = historicWaterUsageOverview };

            return Ok(waterUsageOverviewDto);
        }
        private List<CumulativeWaterUsageByMonthDto> GetHistoricWaterUsageOverview(List<CumulativeWaterUsageByMonthDto> waterUsageOverviewDtos)
        {
            var monthlyWaterUsageOverviewDtos = waterUsageOverviewDtos.GroupBy(x => x.Month).Select(x => new CumulativeWaterUsageByMonthDto
                { Month = x.Key, CumulativeWaterUsageInAcreFeet = Math.Round(x.Where(y => y.CumulativeWaterUsageInAcreFeet.HasValue).Average(y => y.CumulativeWaterUsageInAcreFeet.Value), 1) });

            return monthlyWaterUsageOverviewDtos.ToList();
        }

        private List<CumulativeWaterUsageByMonthDto> GetCurrentWaterUsageOverview(IGrouping<int, ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos)
        {
            var parcelMonthlyEvapotranspirationGroupedByMonth = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterMonth).ToList();
            var monthlyWaterUsageOverviewDtos = new List<CumulativeWaterUsageByMonthDto>();

            decimal cumulativeTotal = 0;

            for (var i = 1; i < 13; i++)
            {
                var grouping = parcelMonthlyEvapotranspirationGroupedByMonth.SingleOrDefault(x => x.Key == i);
                cumulativeTotal += grouping?.Sum(x => x.OverriddenEvapotranspirationRate ?? x.EvapotranspirationRate) ?? 0;
                var monthlyWaterUsageOverviewDto = new CumulativeWaterUsageByMonthDto()
                {
                    Month = ((DateUtilities.Month)i).ToString(),
                    CumulativeWaterUsageInAcreFeet = grouping == null ? (decimal?)null : Math.Round(cumulativeTotal, 1)
                };

                monthlyWaterUsageOverviewDtos.Add(monthlyWaterUsageOverviewDto);
            }

            return monthlyWaterUsageOverviewDtos;
        }

        private List<MonthlyWaterUsageDto> GetMonthlyWaterUsageDtos(
            IGrouping<int, ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos,
            IEnumerable<ParcelDto> parcelDtos)
        {
            var monthlyWaterUsageDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterMonth).OrderBy(x => x.Key).Select(x =>
                    new MonthlyWaterUsageDto()
                    {
                        Month = ((DateUtilities.Month)x.Key).ToString(),
                        WaterUsageByParcel = GetWaterUsageByParcel(x, parcelDtos)
                    })
                .ToList();

            return monthlyWaterUsageDtos;
        }

        private List<ParcelWaterUsageDto> GetWaterUsageByParcel(
            IGrouping<int, ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos,
            IEnumerable<ParcelDto> parcelDtos)
        {
            var parcelWaterUsageDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.ParcelID).Select(groupedByParcel =>
                new ParcelWaterUsageDto
                {
                    ParcelNumber = parcelDtos.Single(parcel => parcel.ParcelID == groupedByParcel.Key).ParcelNumber,
                    WaterUsageInAcreFeet = Math.Round(groupedByParcel.Sum(x => x.OverriddenEvapotranspirationRate ?? x.EvapotranspirationRate), 1)
                }).ToList();

            return parcelWaterUsageDtos;
        }

        private static List<MailMessage> GenerateAddedUserEmails(string rioUrl, AccountDto addedAccount, IEnumerable<UserDto> addedUsers)
        {

            var mailMessages = new List<MailMessage>();
            var messageBody = $@"The following account has been added to your profile in the Rosedale-Rio Bravo Water Accounting Platform. You can now manage this accounts in the Water Accounting Platform:<br/><br/>
{addedAccount.AccountDisplayName}<br/><br/>
You can view parcels associated with this account and the water allocation and usage of those parcels on your <a href='{rioUrl}/landowner-dashboard'>Landowner Dashboard</a>";

            var mailTos = addedUsers;
            foreach(var mailTo in mailTos)
            {
                var mailMessage = new MailMessage
                {
                    Subject = $"Rosedale-Rio Bravo Water Accounting Platform: Water Accounts Added",
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
