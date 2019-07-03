using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Posting;

namespace Rio.API.Controllers
{
    public class PostingTypeController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<PostingTypeController> _logger;
        private readonly KeystoneService _keystoneService;

        public PostingTypeController(RioDbContext dbContext, ILogger<PostingTypeController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpGet("postingTypes")]
        public ActionResult<IEnumerable<PostingTypeDto>> Get()
        {
            var postingTypeDtos = PostingType.List(_dbContext);
            return Ok(postingTypeDtos);
        }
    }
}