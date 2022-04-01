using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;

namespace Rio.API.Controllers
{
    [ApiController]
    public class PostingTypeController : SitkaController<PostingTypeController>
    {
        public PostingTypeController(RioDbContext dbContext, ILogger<PostingTypeController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("postingTypes")]
        public ActionResult<IEnumerable<PostingTypeDto>> Get()
        {
            var postingTypeDtos = PostingType.AllAsDto;
            return Ok(postingTypeDtos);
        }
    }
}