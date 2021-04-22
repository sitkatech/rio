using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class AccountReconciliationController : SitkaController<AccountReconciliationController>
    {

        public AccountReconciliationController(RioDbContext dbContext, ILogger<AccountReconciliationController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
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