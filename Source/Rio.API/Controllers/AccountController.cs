using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Account;

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

        [HttpGet("/accounts")]
        [UserManageFeature]
        public ActionResult<List<AccountDto>> ListAllAccounts()
        {
            var accountDtos = Account.List(_dbContext);
            return accountDtos;
        }
    }
}
