using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;

namespace Rio.API.Controllers
{
    [ApiController]
    public class OpenETController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<OpenETController> _logger;
        private readonly KeystoneService _keystoneService;
        private readonly RioConfiguration _rioConfiguration;

        public const string OpenETBucketURL =
            "https://storage.googleapis.com/openet_raster_api_storage/openet_timeseries_multi_output/testing_multi_mean_API_KEY.csv";

        public const string TriggerTimeSeriesURL =
            "http://3.228.142.200/timeseries_multipolygon?shapefile_fn=projects/openet/featureCollections/Use_Case_Data/Rosedale-RioBravoWSD/RRBWSD_2019parcels_wgs84&start_date=START_DATE&end_date=END_DATE&model=sims&vars=et&aggregation_type=mean&api_key=API_KEY&to_cloud=openet_raster_api_storage";

        public OpenETController(RioDbContext dbContext, ILogger<OpenETController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _keystoneService = keystoneService;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpGet("triggerOpenETRetrieveJob")]
        public ActionResult TriggerOpenETJob()
        {
            var response = OpenETGoogleBucketHelpers.TriggerOpenETGoogleBucketRefresh(_rioConfiguration,  "2016-01-01", "2016-12-31");
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(response.ReasonPhrase);
            }

            Task.Delay(TimeSpan.FromMinutes(15)).ContinueWith(_ =>
                OpenETGoogleBucketHelpers.UpdateParcelMonthlyEvapotranspirationWithETData(_dbContext,
                    _rioConfiguration));
            return Ok();
        }
    }
}
