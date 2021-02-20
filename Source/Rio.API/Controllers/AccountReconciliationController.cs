using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Account;

namespace Rio.API.Controllers
{
    [ApiController]
    public class AccountReconciliationController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<AccountReconciliationController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;

        public AccountReconciliationController(RioDbContext dbContext, ILogger<AccountReconciliationController> logger,
            KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpGet("/account-reconciliations")]
        [ManagerDashboardFeature]
        public ActionResult<List<AccountReconciliationDto>> ListAllAccounts()
        {
            var accountReconciliationDtos = AccountReconciliation.List(_dbContext);
            return accountReconciliationDtos;
        }
    }
}