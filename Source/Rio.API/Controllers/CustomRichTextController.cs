﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.Parcel;

namespace Rio.API.Controllers
{
    [ApiController]
    public class CustomRichTextController
        : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<RoleController> _logger;
        private readonly KeystoneService _keystoneService;

        public CustomRichTextController(RioDbContext dbContext, ILogger<RoleController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }



        [HttpGet("customRichText/{customRichTextTypeID}")]
        public ActionResult<CustomRichTextDto> GetCustomRichText([FromRoute] int customRichTextTypeID)
        {
            var customRichTextDto = CustomRichText.GetByCustomRichTextTypeID(_dbContext, customRichTextTypeID);
            if (customRichTextDto == null)
            {
                return NotFound();
            }

            return Ok(customRichTextDto);
        }

        [HttpPut("customRichText/{customRichTextTypeID}")]
        [ContentManageFeature]
        public ActionResult<CustomRichTextDto> UpdateCustomRichText([FromRoute] int customRichTextTypeID,
            [FromBody] CustomRichTextDto customRichTextUpdateDto)
        {
            var customRichTextDto = CustomRichText.GetByCustomRichTextTypeID(_dbContext, customRichTextTypeID);
            if (customRichTextDto == null)
            {
                return NotFound();
            }

            CustomRichTextDto updatedCustomRichTextDto =
                CustomRichText.UpdateCustomRichText(_dbContext, customRichTextTypeID, customRichTextUpdateDto);

            return Ok(updatedCustomRichTextDto);
        }
    }
}