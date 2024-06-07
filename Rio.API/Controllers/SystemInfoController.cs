using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;
using System;
using Rio.Models.DataTransferObjects;
using Zybach.API.Logging;

namespace Rio.API.Controllers;

[ApiController]
public class SystemInfoController : SitkaController<SystemInfoController>
{
    public SystemInfoController(RioDbContext dbContext, ILogger<SystemInfoController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
    {
    }

    [HttpGet("/", Name = "GetSystemInfo")]  // MCS: the pattern seems to be to allow anonymous access to this endpoint
    [AllowAnonymous]
    [LogIgnore]
    public ActionResult<SystemInfoDto> GetSystemInfo([FromServices] IWebHostEnvironment environment)
    {
        SystemInfoDto systemInfo = new SystemInfoDto
        {
            Environment = environment.EnvironmentName,
            CurrentTimeUTC = DateTime.UtcNow.ToString("o"),
            PodName = _rioConfiguration.HostName
        };
        return Ok(systemInfo);
    }
}