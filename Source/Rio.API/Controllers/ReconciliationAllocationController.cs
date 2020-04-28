using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.ReconciliationAllocation;

namespace Rio.API.Controllers
{
    public class ReconciliationAllocationController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly RioConfiguration _rioConfiguration;

        public ReconciliationAllocationController(RioDbContext dbContext, ILogger<AccountController> logger, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpPost("reconciliationAllocation/upload")]
        public async Task<ActionResult> Upload()
        {
            var fileResource = await HttpUtilities.MakeFileResourceFromHttpRequest(Request, _dbContext, HttpContext);

            _dbContext.FileResource.Add(fileResource);
            _dbContext.SaveChanges();

            var reconciliationAllocationUpload = new ReconciliationAllocationUpload()
            {
                FileResourceID = fileResource.FileResourceID,
                ReconciliationAllocationUploadStatusID = (int) ReconciliationAllocationUploadStatusEnum.Pending,
                UploadUserID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID
            };

            _dbContext.ReconciliationAllocationUpload.Add(reconciliationAllocationUpload);
            _dbContext.SaveChanges();

            return Ok(new ReconciliationAllocationUploadConfirmDto(){ReconciliationAllocationUploadID = reconciliationAllocationUpload.ReconciliationAllocationUploadID});
        }
    }
}
