using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Account;
using System.Collections.Generic;

namespace Rio.API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly KeystoneService _keystoneService;

        public AccountController(RioDbContext dbContext, ILogger<AccountController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
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
                return NotFound($"Could not find a System AccountStatus with the ID {accountUpdateDto.AccountStatusID}");
            }

            var updatedUserDto = Account.CreateAccountEntity(_dbContext, accountUpdateDto);
            return Ok(updatedUserDto);
        }
    }
}
