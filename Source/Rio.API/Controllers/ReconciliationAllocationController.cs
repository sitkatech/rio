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

        [HttpPost("reconciliationAllocation/upload/{waterYear}")]
        public async Task<ActionResult> Upload([FromRoute] int waterYear)
        {
            List<ReconciliationAllocationCSV> records;

            var fileResource = await HttpUtilities.MakeFileResourceFromHttpRequest(Request, _dbContext, HttpContext);

            if (!ParseReconciliationAllocationUpload(fileResource, out records, out var badRequestFromUpload))
            {
                return badRequestFromUpload;
            }

            if (ValidateReconciliationAllocationUpload(records, out var badRequestFromValidation))
            {
                return badRequestFromValidation;
            }

            _dbContext.FileResource.Add(fileResource);
            _dbContext.SaveChanges();

            //var reconciliationAllocationUpload = new ReconciliationAllocationUpload()
            //{
            //    FileResourceID = fileResource.FileResourceID,
            //    ReconciliationAllocationUploadStatusID = (int)ReconciliationAllocationUploadStatusEnum.Pending,
            //    UploadUserID = UserContext.GetUserFromHttpContext(_dbContext, HttpContext).UserID
            //};

            //_dbContext.ReconciliationAllocationUpload.Add(reconciliationAllocationUpload);
            //_dbContext.SaveChanges();

            ParcelAllocationHistory.CreateParcelAllocationHistoryEntity(_dbContext,
                UserContext.GetUserFromHttpContext(_dbContext,HttpContext).UserID, fileResource.FileResourceID, waterYear,
                (int) ParcelAllocationTypeEnum.Reconciliation, null);

            Account.SetReconciliationAllocation(_dbContext, records, waterYear);

            return Ok();
        }

        private bool ParseReconciliationAllocationUpload(FileResource fileResource, out List<ReconciliationAllocationCSV> records, out ActionResult badRequest)
        {
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
                {
                    badRequest = BadRequest(new
                        {validationMessage = "Could not parse CSV file. Check that no rows are blank or missing data."});
                    records = null;
                    return false;
                }
            }

            badRequest = null;
            return true;
        }

        private bool ValidateReconciliationAllocationUpload(List<ReconciliationAllocationCSV> records, out ActionResult badRequest)
        {
// validate: no account number duplicated.
            var duplicateAccountNumbers =
                records.GroupBy(x => x.AccountNumber).Where(x => x.Count() > 1).Select(x => x.Key).ToList();
            if (duplicateAccountNumbers.Any())
            {
                {
                    badRequest = BadRequest(new
                    {
                        validationMessage =
                            "The upload contained multiples rows with these account numbers: " +
                            string.Join(", ", duplicateAccountNumbers)
                    });
                    return true;
                }
            }

            // validate: all account numbers match.
            var allAccountNumbers = _dbContext.Account.Select(y => y.AccountNumber);
            var unmatchedRecords = records.Where(x => !allAccountNumbers.Contains(x.AccountNumber)).ToList();
            if (unmatchedRecords.Any())
            {
                {
                    badRequest = BadRequest(new
                    {
                        validationMessage =
                            "The upload contained these account numbers which did not match any record in the system: " +
                            string.Join(", ", unmatchedRecords.Select(x => x.AccountNumber))
                    });
                    return true;
                }
            }

            badRequest = null;

            return false;
        }
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
