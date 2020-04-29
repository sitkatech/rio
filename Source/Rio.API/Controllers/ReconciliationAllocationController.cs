using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.ReconciliationAllocation;

namespace Rio.API.Controllers
{
    public class ReconciliationAllocationController : ControllerBase
    {
        private readonly RioDbContext _dbContext;
        private readonly ILogger<AccountController> _logger;
        private readonly RioConfiguration _rioConfiguration;

        public ReconciliationAllocationController(RioDbContext dbContext, ILogger<AccountController> logger, IOptions<RioConfiguration> rioConfiguration)
        {
            _dbContext = dbContext;
            _logger = logger;
            _rioConfiguration = rioConfiguration.Value;
        }

        [HttpPost("reconciliationAllocation/upload")]
        public async Task<ActionResult> Upload()
        {
            List<ReconciliationAllocationCSV> records;

            var fileResource = await HttpUtilities.MakeFileResourceFromHttpRequest(Request, _dbContext, HttpContext);

            try
            {
                using (var memoryStream = new MemoryStream(fileResource.FileResourceData))
                using (var reader = new StreamReader(memoryStream))
                using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csvReader.Configuration.RegisterClassMap<ReconciliationAllocationCSVMap>();
                    records = csvReader.GetRecords<ReconciliationAllocationCSV>().ToList();
                }
            }
            catch
            {
                return BadRequest(new { validationMessage = "Could not parse CSV file. Check that no rows are blank or missing data." });
            }

            // validate: no account number duplicated.
            var duplicateAccountNumbers = records.GroupBy(x => x.AccountNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicateAccountNumbers.Any())
            {
                return BadRequest(new
                {
                    validationMessage = 
                        "The upload contained multiples rows with these account numbers: " +
                        string.Join(", ", duplicateAccountNumbers)
                });
            }

            // validate: all account numbers match.
            var allAccountNumbers = _dbContext.Account.Select(y => y.AccountNumber);
            var unmatchedRecords = records.Where(x => !allAccountNumbers.Contains(x.AccountNumber)).ToList();
            if (unmatchedRecords.Any())
            {
                return BadRequest(new
                {
                    validationMessage =
                        "The upload contained these account numbers which did not match any record in the system: " +
                        string.Join(", ", unmatchedRecords.Select(x => x.AccountNumber))
                });
            }

            _dbContext.FileResource.Add(fileResource);
            _dbContext.SaveChanges();

            var reconciliationAllocationUpload = new ReconciliationAllocationUpload()
            {
                FileResourceID = fileResource.FileResourceID,
                ReconciliationAllocationUploadStatusID = (int)ReconciliationAllocationUploadStatusEnum.Pending,
                UploadUserID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID
            };

            _dbContext.ReconciliationAllocationUpload.Add(reconciliationAllocationUpload);
            _dbContext.SaveChanges();

            return Ok(new ReconciliationAllocationUploadConfirmDto() { ReconciliationAllocationUploadID = reconciliationAllocationUpload.ReconciliationAllocationUploadID });
        }
    }

    public class ReconciliationAllocationCSV
    {
        public int AccountNumber { get; set; }
        public double ReconciliationVolume { get; set; }
    }

    public sealed class ReconciliationAllocationCSVMap : ClassMap<ReconciliationAllocationCSV>
    {
        public ReconciliationAllocationCSVMap()
        {
            Map(m => m.AccountNumber).Name("Account Number");
            Map(m => m.ReconciliationVolume).Name("Reconciliation Volume");
        }
    }
}
