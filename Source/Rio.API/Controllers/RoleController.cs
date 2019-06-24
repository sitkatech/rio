﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Role;

namespace Rio.API.Controllers
{
    public class RoleController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<RoleController> _logger;
        private readonly KeystoneService _keystoneService;

        public RoleController(RioDbContext dbContext, ILogger<RoleController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpGet("roles")]
        public ActionResult<IEnumerable<RoleDto>> Get()
        {
            var roleDtos = Role.GetList(_dbContext);
            return Ok(roleDtos);
        }
    }
}