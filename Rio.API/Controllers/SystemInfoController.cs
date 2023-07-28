using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;
using System;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers;

[ApiController]
public class SystemInfoController : SitkaController<SystemInfoController>
{
    public SystemInfoController(RioDbContext dbContext, ILogger<SystemInfoController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
    {
    }

    [HttpGet("/", Name = "GetSystemInfo")]  // MCS: the pattern seems to be to allow anonymous access to this endpoint
    [AllowAnonymous]
    public IActionResult GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        SystemInfoDto systemInfo = new SystemInfoDto
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o")
        };

        return Ok(systemInfo);
    }

}