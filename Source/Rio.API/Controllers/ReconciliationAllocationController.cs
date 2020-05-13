using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Rio.API.Services;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.ReconciliationAllocation;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Rio.API.Controllers
{
    public class ReconciliationAllocationController : ControllerBase
    {
        private readonly RioDbContext _dbContext;

        public ReconciliationAllocationController(RioDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("reconciliationAllocation/upload/{waterYear}")]
        public async Task<ActionResult> Upload([FromRoute] int waterYear)
        {
            var fileResource = await HttpUtilities.MakeFileResourceFromHttpRequest(Request, _dbContext, HttpContext);

            if (!ParseReconciliationAllocationUpload(fileResource, out var records, out var badRequestFromUpload))
            {
                return badRequestFromUpload;
            }

            if (!ValidateReconciliationAllocationUpload(records, out var badRequestFromValidation))
            {
                return badRequestFromValidation;
            }

            _dbContext.FileResource.Add(fileResource);
            _dbContext.SaveChanges();

            //ParcelAllocation.SetReconciliationAllocation(_dbContext, records, waterYear);
            
            ParcelAllocationHistory.CreateParcelAllocationHistoryEntity(_dbContext,
                UserContext.GetUserFromHttpContext(_dbContext,HttpContext).UserID, fileResource.FileResourceID, waterYear,
                (int) ParcelAllocationTypeEnum.Reconciliation, null);

            return Ok();
        }

        private bool ParseReconciliationAllocationUpload(FileResource fileResource, out List<ReconciliationAllocationCSV> records, out ActionResult badRequest)
        {
            try
            {
                using var memoryStream = new MemoryStream(fileResource.FileResourceData);
                using var reader = new StreamReader(memoryStream);
                using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
                csvReader.Configuration.RegisterClassMap<ReconciliationAllocationCSVMap>();
                csvReader.Read();
                csvReader.ReadHeader();
                var headerNamesDuplicated = csvReader.Context.HeaderRecord.GroupBy(x => x).Where(x => x.Count() > 1).ToList();
                if (headerNamesDuplicated.Any())
                {
                    badRequest = BadRequest(new
                    {
                        validationMessage =
                            $"The following header names appear more than once: {string.Join(", ", headerNamesDuplicated.OrderBy(x => x.Key).Select(x => x.Key))}"
                    });
                    records = null;
                    return false;
                }
                records = csvReader.GetRecords<ReconciliationAllocationCSV>().ToList();
            }
            catch (HeaderValidationException e)
            {
                var headerMessage = e.Message.Split('.')[0];
                badRequest = BadRequest(new
                {
                    validationMessage =
                        $"{headerMessage}. Please check that the column name is not missing or misspelled."
                });
                records = null;
                return false;
            }
            catch
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                       "There was an error parsing the CSV. Please ensure the file was formatted correctly."
                });
                records = null;
                return false;
            }

            badRequest = null;
            return true;
        }

        private bool ValidateReconciliationAllocationUpload(List<ReconciliationAllocationCSV> records, out ActionResult badRequest)
        {
            // no null recvol
            var nullReconciliationVolumes = records.Where(x=>x.ReconciliationVolume == null).ToList();
            if (nullReconciliationVolumes.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        "The following Account Numbers had no Reconciliation Volume entered: " +
                        string.Join(", ", nullReconciliationVolumes.Select(x => x.AccountNumber))
                });
                return false;
            }

            // no duplicate account numbers permitted
            var duplicateAccountNumbers = records.GroupBy(x => x.AccountNumber).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToList();

            if (duplicateAccountNumbers.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        "The upload contained multiples rows with these account numbers: " +
                        string.Join(", ", duplicateAccountNumbers)
                });
                return false;
            }

            // all account numbers must match
            var allAccountNumbers = _dbContext.Account.Select(y => y.AccountNumber);
            var unmatchedRecords = records.Where(x => !allAccountNumbers.Contains(x.AccountNumber)).ToList();

            if (unmatchedRecords.Any())
            {
                badRequest = BadRequest(new
                {
                    validationMessage =
                        "The upload contained these account numbers which did not match any record in the system: " +
                        string.Join(", ", unmatchedRecords.Select(x => x.AccountNumber))
                });
                return false;
            }

            badRequest = null;
            return true;
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
