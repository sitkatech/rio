using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;

namespace Rio.API.Controllers
{
    [ApiController]
    public class RoleController : SitkaController<RoleController>
    {
        public RoleController(RioDbContext dbContext, ILogger<RoleController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }

        [HttpGet("roles")]
        [UserManageFeature]
        public IActionResult Get()
        {
            var roleDtos = Role.List(_dbContext);
            return Ok(roleDtos);
        }
    }
}