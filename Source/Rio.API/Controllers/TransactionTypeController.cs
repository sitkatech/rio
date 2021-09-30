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
    public class TransactionTypeController : SitkaController<TransactionTypeController>
    {
        public TransactionTypeController(RioDbContext dbContext, ILogger<TransactionTypeController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("/transaction-types/")]
        [LoggedInUnclassifiedFeature]
        public ActionResult<List<TransactionTypeDto>> GetTransactionTypes()
        {
            var transactionTypeDtos = TransactionType.GetTransactionTypes(_dbContext);
            return Ok(transactionTypeDtos);
        }
        
        [HttpGet("/allocation-types/")]
        [LoggedInUnclassifiedFeature]
        public ActionResult<List<TransactionTypeDto>> GetAllocationTypes()
        {
            var transactionTypeDtos = TransactionType.GetAllocationTypes(_dbContext);
            return Ok(transactionTypeDtos);
        }
    }
}