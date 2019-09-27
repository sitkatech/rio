using System;
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
using Rio.Models.DataTransferObjects.Posting;
using Rio.Models.DataTransferObjects.User;
using Rio.Models.DataTransferObjects.WaterTransfer;
using Rio.Models.DataTransferObjects;
using Rio.Models.DataTransferObjects.WaterUsage;

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
            var existingUser = EFModels.Entities.User.GetByEmail(_dbContext, inviteDto.Email);
            if (existingUser != null)
            {
                existingUser = EFModels.Entities.User.UpdateUserGuid(_dbContext, existingUser.UserID, keystoneUser.UserGuid);
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
            var userDtos = EFModels.Entities.User.List(_dbContext);
            return Ok(userDtos);
        }

        [HttpGet("users/{userID}")]
        [UserViewFeature]
        public ActionResult<UserDto> GetByUserID([FromRoute] int userID)
        {
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
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
            var userDto = EFModels.Entities.User.GetByUserID(_dbContext, userID);
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
        [UserViewFeature]
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
        [UserViewFeature]
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
            var waterYears = DateUtilities.GetRangeOfYears(DateUtilities.MinimumYear, DateUtilities.GetLatestWaterYear());
            var parcelAllocationAndConsumptionDtos = ParcelExtensionMethods.CreateParcelAllocationAndConsumptionDtos(waterYears, parcelDtos, parcelAllocationDtos, parcelMonthlyEvapotranspirationDtos);
            return Ok(parcelAllocationAndConsumptionDtos);
        }

        [HttpGet("users/{userID}/postings")]
        [UserViewFeature]
        public ActionResult<List<PostingDto>> ListPostingsByUserID([FromRoute] int userID)
        {
            var postingDtos = Posting.ListByUserID(_dbContext, userID);
            if (postingDtos == null)
            {
                return NotFound();
            }

            return Ok(postingDtos);
        }

        [HttpGet("users/{userID}/water-transfers")]
        [UserViewFeature]
        public ActionResult<List<WaterTransferDto>> ListWaterTransfersByUserID([FromRoute] int userID)
        {
            var waterTransferDtos = WaterTransfer.ListByUserID(_dbContext, userID);
            if (waterTransferDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferDtos);
        }

        [HttpGet("users/{userID}/water-usage")]
        [UserViewFeature]
        public ActionResult<List<WaterUsageByParcelDto>> ListWaterUsagesByUserID([FromRoute] int userID)
        {
            var parcelDtos = Parcel.ListByUserID(_dbContext, userID);
            var parcelIDs = parcelDtos.Select(x=>x.ParcelID).ToList();

            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelIDs);

            var waterUsageDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterYear).Select(x =>
                new WaterUsageByParcelDto {Year = x.Key, WaterUsage = GetMonthlyWaterUsageDtos(x, parcelDtos)});

            return Ok(waterUsageDtos);
        }

        private List<MonthlyWaterUsageDto> GetMonthlyWaterUsageDtos(
            IGrouping<int, ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos,
            IEnumerable<ParcelDto> parcelDtos)
        {
            var monthlyWaterUsageDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterMonth).OrderBy(x => x.Key).Select(x =>
                    new MonthlyWaterUsageDto()
                    {
                        Month = ((DateUtilities.Month) x.Key).ToString(), WaterUsageByParcel = GetWaterUsageByParcel(x, parcelDtos)
                    })
                .ToList();

            return monthlyWaterUsageDtos;
        }

        private List<ParcelWaterUsageDto> GetWaterUsageByParcel(
            IGrouping<int, ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos,
            IEnumerable<ParcelDto> parcelDtos)
        {
            var parcelWaterUsageDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.ParcelID).Select(groupedByParcel =>
                new ParcelWaterUsageDto
                {
                    ParcelNumber = parcelDtos.Single(parcel => parcel.ParcelID == groupedByParcel.Key).ParcelNumber,
                    WaterUsageInAcreFeet = Math.Round(groupedByParcel.Sum(x => x.EvapotranspirationRate), 1)
                }).ToList();

            return parcelWaterUsageDtos;
        }

        [HttpGet("users/{userID}/water-usage-overview")]
        [UserViewFeature]
        public ActionResult<WaterUsageOverviewDto> GetWaterUsageOverviewByUserID([FromRoute] int userID)
        {
            var parcelDtos = Parcel.ListByUserID(_dbContext, userID);
            var parcelIDs = parcelDtos.Select(x => x.ParcelID).ToList();

            var parcelMonthlyEvapotranspirationDtos = ParcelMonthlyEvapotranspiration.ListByParcelID(_dbContext, parcelIDs).ToList();

            var cumulativeWaterUsageByYearDtos = parcelMonthlyEvapotranspirationDtos.GroupBy(x => x.WaterYear).Select(x => new CumulativeWaterUsageByYearDto
                {Year = x.Key, CumulativeWaterUsage = GetCurrentWaterUsageOverview(x)}).ToList();

            var historicWaterUsageOverview = GetHistoricWaterUsageOverview(cumulativeWaterUsageByYearDtos.SelectMany(x => x.CumulativeWaterUsage).ToList());

            // the chart needs value to be non null, so we need to set the cumulativewaterusage values to be 0 for the null ones; we need them to be null originally when calculating historic since we don't want them to count
            foreach (var cumulativeWaterUsageByMonthDto in cumulativeWaterUsageByYearDtos.SelectMany(x => x.CumulativeWaterUsage).Where(y => y.CumulativeWaterUsageInAcreFeet == null))
            {
                cumulativeWaterUsageByMonthDto.CumulativeWaterUsageInAcreFeet = 0;
            }

            var waterUsageOverviewDto = new WaterUsageOverviewDto { Current = cumulativeWaterUsageByYearDtos, Historic = historicWaterUsageOverview};

            return Ok(waterUsageOverviewDto);
        }

        private List<CumulativeWaterUsageByMonthDto> GetHistoricWaterUsageOverview(List<CumulativeWaterUsageByMonthDto> waterUsageOverviewDtos)
        {
            var monthlyWaterUsageOverviewDtos = waterUsageOverviewDtos.GroupBy(x => x.Month).Select(x => new CumulativeWaterUsageByMonthDto
                {Month = x.Key, CumulativeWaterUsageInAcreFeet = Math.Round(x.Where(y => y.CumulativeWaterUsageInAcreFeet.HasValue).Average(y => y.CumulativeWaterUsageInAcreFeet.Value), 1)});

            return monthlyWaterUsageOverviewDtos.ToList();
        }

        private List<CumulativeWaterUsageByMonthDto> GetCurrentWaterUsageOverview(IGrouping<int, ParcelMonthlyEvapotranspirationDto> parcelMonthlyEvapotranspirationDtos)
        {
            var parcelMonthlyEvapotranspirationGroupedByMonth = parcelMonthlyEvapotranspirationDtos.GroupBy(x=>x.WaterMonth).ToList();
            var monthlyWaterUsageOverviewDtos = new List<CumulativeWaterUsageByMonthDto>();

            decimal cumulativeTotal = 0;

            for (var i = 1; i < 13; i++)
            {
                var grouping = parcelMonthlyEvapotranspirationGroupedByMonth.SingleOrDefault(x => x.Key == i);
                cumulativeTotal += grouping?.Sum(x => x.EvapotranspirationRate) ?? 0;
                var monthlyWaterUsageOverviewDto = new CumulativeWaterUsageByMonthDto()
                {
                    Month = ((DateUtilities.Month) i).ToString(),
                    CumulativeWaterUsageInAcreFeet = grouping == null ? (decimal?) null : Math.Round(cumulativeTotal, 1)
                };

                monthlyWaterUsageOverviewDtos.Add(monthlyWaterUsageOverviewDto);
            }

            return monthlyWaterUsageOverviewDtos;
        }
    }
}