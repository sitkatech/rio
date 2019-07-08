﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.API.Services.Filter;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Parcel;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Controllers
{
    public class UserController : Controller
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<UserController> _logger;
        private readonly KeystoneService _keystoneService;

        public UserController(RioDbContext dbContext, ILogger<UserController> logger, KeystoneService keystoneService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
        }

        [HttpPost("/users/invite")]
        [UserManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid User Invite JSON object from the Request Body.")]
        public IActionResult InviteUser([FromBody] UserInviteDto inviteDto)
        {
            if (inviteDto.RoleID.HasValue)
            {
                var role = Role.GetByRoleID(_dbContext, inviteDto.RoleID.Value);
                if (role == null)
                {
                    return NotFound($"Could not find a Role with the ID {inviteDto.RoleID}");
                }
            }
            else
            {
                return BadRequest($"Role ID is required.");
            }

            const string applicationName = "Rosedale-Rio Bravo Water Trading Platform";
            const string rioBravoWaterStorageDistrict = "Rosedale-Rio Bravo Water Storage District";
            var inviteModel = new KeystoneService.KeystoneInviteModel
            {
                FirstName = inviteDto.FirstName,
                LastName = inviteDto.LastName,
                Email = inviteDto.Email,
                Subject = $"Invitation to the {applicationName}",
                WelcomeText = $"You are receiving this notification because you are identified as a participant in the pilot phase of the {applicationName}, an online platform provided by the {rioBravoWaterStorageDistrict}.",
                SiteName = applicationName,
                SignatureBlock = $"{rioBravoWaterStorageDistrict}<br /><a href='mailto:admin@rrbwsd.com'>admin@rrbwsd.com</a><br />(661) 589-6045<br /><a href='https://www.rrbwsd.com'>https://www.rrbwsd.com</a>",
                RedirectURL = "https://rrbwatertrading.sitkatech.com"
            };

            var response = _keystoneService.Invite(inviteModel);
            if (response.StatusCode != HttpStatusCode.OK || response.Error != null)
            {
                ModelState.AddModelError("Email", $"There was a problem inviting the user to Keystone: {response.Error.Message}.");
                if (response.Error.ModelState != null)
                {
                    foreach (var modelStateKey in response.Error.ModelState.Keys)
                    {
                        foreach (var err in response.Error.ModelState[modelStateKey])
                        {
                            ModelState.AddModelError(modelStateKey, err);
                        }
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var keystoneUser = response.Payload.Claims;
            var existingUser = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, keystoneUser.UserGuid);
            if (existingUser != null)
            {
                return Ok(existingUser);
            }

            var newUser = new UserUpsertDto
            {
                FirstName = keystoneUser.FirstName,
                LastName = keystoneUser.LastName,
                OrganizationName = keystoneUser.OrganizationName,
                Email = keystoneUser.Email,
                PhoneNumber = keystoneUser.PrimaryPhone,
                RoleID = inviteDto.RoleID.Value
            };

            var user = Rio.EFModels.Entities.User.CreateNewUser(_dbContext, newUser, keystoneUser.LoginName, keystoneUser.UserGuid);
            return Ok(user);
        }

        [HttpGet("users")]
        [UserManageFeature]
        public ActionResult<IEnumerable<UserDto>> List()
        {
            var userDtos = Rio.EFModels.Entities.User.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/{userID}")]
        [UserManageFeature]
        public ActionResult<UserDto> GetByUserID([FromRoute] int userID)
        {
            var userDto = Rio.EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }

        [HttpGet("user-claims/{globalID}")]
        public ActionResult<UserDto> GetByGlobalID([FromRoute] string globalID)
        {
            var isValidGuid = Guid.TryParse(globalID, out var globalIDAsGuid);
            if (!isValidGuid)
            {
                return BadRequest();
            }

            var userDto = Rio.EFModels.Entities.User.GetByUserGuid(_dbContext, globalIDAsGuid);
            if (userDto == null)
            {
                return NotFound();
            }

            return Ok(userDto);
        }

        [HttpPut("users/{userID}")]
        [UserManageFeature]
        [RequiresValidJSONBodyFilter("Could not parse a valid User Upsert JSON object from the Request Body.")]
        public ActionResult<UserDto> UpdateUser([FromRoute] int userID, [FromBody] UserUpsertDto userUpsertDto)
        {
            var userDto = Rio.EFModels.Entities.User.GetByUserID(_dbContext, userID);
            if (userDto == null)
            {
                return NotFound();
            }

            var validationMessages = Rio.EFModels.Entities.User.ValidateUpdate(_dbContext, userUpsertDto, userDto.UserID);
            validationMessages.ForEach(vm => {
                ModelState.AddModelError(vm.Type, vm.Message);
            });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = Role.GetByRoleID(_dbContext, userUpsertDto.RoleID.Value);
            if (role == null)
            {
                return NotFound($"Could not find a System Role with the ID {userUpsertDto.RoleID}");
            }

            var updatedUserDto = Rio.EFModels.Entities.User.UpdateUserEntity(_dbContext, userID, userUpsertDto);
            return Ok(updatedUserDto);
        }


        [HttpGet("users/{userID}/parcels")]
        [UserManageFeature]
        public ActionResult<List<ParcelDto>> ListParcelsByUserID([FromRoute] int userID)
        {
            var parcelDtos = Parcel.ListByUserID(_dbContext, userID);
            if (parcelDtos == null)
            {
                return NotFound();
            }

            return Ok(parcelDtos);
        }

        [HttpGet("users/{userID}/getParcelsAllocationAndConsumption")]
        [UserManageFeature]
        public ActionResult<List<ParcelAllocationAndConsumptionDto>> ListParcelsAllocationAndConsumptionByUserID([FromRoute] int userID)
        {
            var parcelDtosEnumerable = Parcel.ListByUserID(_dbContext, userID);
            if (parcelDtosEnumerable == null)
            {
                return NotFound();
            }

            var parcelDtos = parcelDtosEnumerable.ToList();
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();
            var parcelAllocationDtos = ParcelAllocation.ListByParcelID(_dbContext, parcelIDs);
            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelIDs);
            var waterYears = DateUtilities.GetRangeOfYears(DateUtilities.MinimumYear, DateTime.Now.Year);
            var parcelAllocationAndConsumptionDtos = ParcelExtensionMethods.CreateParcelAllocationAndConsumptionDtos(waterYears, parcelDtos, parcelAllocationDtos, parcelMonthlyEvapotranspirationDtos);
            return Ok(parcelAllocationAndConsumptionDtos);
        }
    }
}