﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Rio.API.Services;
using Rio.API.Services.Authorization;
using Rio.EFModels.Entities;
using Rio.Models.DataTransferObjects.Account;
using Rio.Models.DataTransferObjects.WaterTransfer;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Rio.API.Controllers
{
    [ApiController]
    public class WaterTransferController : SitkaController<WaterTransferController>
    {
        public WaterTransferController(RioDbContext dbContext, ILogger<WaterTransferController> logger, KeystoneService keystoneService, IOptions<RioConfiguration> rioConfiguration) : base(dbContext, logger, keystoneService, rioConfiguration)
        {
        }


        [HttpGet("water-transfers/{waterTransferID}")]
        [OfferManageFeature]
        public ActionResult<WaterTransferDto> GetByWaterTransferID([FromRoute] int waterTransferID)
        {
            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            return RequireNotNullThrowNotFound(waterTransferDto, "Water Transfer", waterTransferID);
        }

        [HttpGet("water-transfers/{waterTransferID}/registrations")]
        [OfferManageFeature]
        public ActionResult<List<WaterTransferRegistrationSimpleDto>> GetWaterTransferRegistrationsByWaterTransferID([FromRoute] int waterTransferID)
        {
            var waterTransferRegistrationSimpleDtos = WaterTransferRegistration.GetByWaterTransferID(_dbContext, waterTransferID);
            if (waterTransferRegistrationSimpleDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferRegistrationSimpleDtos);
        }

        [HttpPost("water-transfers/{waterTransferID}/register")]
        [OfferManageFeature]
        public ActionResult<WaterTransferDto> ConfirmTransfer([FromRoute] int waterTransferID, [FromBody] WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (ThrowNotFound(waterTransferDto, "Water Transfer", waterTransferID, out var actionResult))
            {
                return actionResult;
            }

            var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            var validationMessages = WaterTransfer.ValidateConfirmTransfer(waterTransferRegistrationDto, waterTransferDto, currentUser);
            validationMessages.ForEach(vm => {ModelState.AddModelError(vm.Type, vm.Message);});
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            waterTransferDto = WaterTransfer.ChangeWaterRegistrationStatus(_dbContext, waterTransferID, waterTransferRegistrationDto, WaterTransferRegistrationStatusEnum.Registered);
            if (waterTransferDto.BuyerRegistration.IsRegistered && waterTransferDto.SellerRegistration.IsRegistered)
            {
                // create a parcel ledger entry since the water transfer has been confirmed by both parties
                var tradePurchaseParcelLedgers = CreateParcelLedgersFromWaterTransferRegistration(waterTransferDto, TransactionTypeEnum.TradePurchase, waterTransferDto.BuyerRegistration);
                var tradeSaleParcelLedgers = CreateParcelLedgersFromWaterTransferRegistration(waterTransferDto, TransactionTypeEnum.TradeSale, waterTransferDto.SellerRegistration);
                _dbContext.ParcelLedgers.AddRange(tradePurchaseParcelLedgers);
                _dbContext.ParcelLedgers.AddRange(tradeSaleParcelLedgers);
                _dbContext.SaveChanges();

                var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
                var mailMessages = GenerateConfirmTransferEmail(_rioConfiguration.WEB_URL, waterTransferDto, smtpClient);
                foreach (var mailMessage in mailMessages)
                {
                    SendEmailMessage(smtpClient, mailMessage);
                }
            }

            return Ok(waterTransferDto);
        }

        private IEnumerable<ParcelLedger> CreateParcelLedgersFromWaterTransferRegistration(WaterTransferDto waterTransferDto, TransactionTypeEnum transactionTypeEnum, WaterTransferRegistrationSimpleDto waterTransferRegistrationSimpleDto)
        {
            var waterTransferRegistrationParcelDtos =
                WaterTransferRegistrationParcel.ListByWaterTransferRegistrationID(_dbContext,
                    waterTransferRegistrationSimpleDto.WaterTransferRegistrationID);
            var parcelLedgers = waterTransferRegistrationParcelDtos.Select(waterTransferRegistrationParcelDto =>
                    new ParcelLedger
                    {
                        ParcelID = waterTransferRegistrationParcelDto.ParcelID,
                        TransactionAmount = transactionTypeEnum == TransactionTypeEnum.TradePurchase ? waterTransferRegistrationParcelDto.AcreFeetTransferred : -waterTransferRegistrationParcelDto.AcreFeetTransferred,
                        TransactionTypeID = (int)transactionTypeEnum,
                        TransactionDate = DateTime.UtcNow,
                        EffectiveDate = waterTransferDto.TransferDate,
                        TransactionDescription = $"Water from Trade {waterTransferDto.TradeNumber} has been {(transactionTypeEnum == TransactionTypeEnum.TradePurchase ? "deposited" : "withdrawn")} into this water account"
                    })
                .ToList();
            return parcelLedgers;
        }

        [HttpPost("/water-transfers/{waterTransferID}/cancel")]
        [OfferManageFeature]
        public IActionResult CancelTrade([FromRoute] int waterTransferID, [FromBody] WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (ThrowNotFound(waterTransferDto, "Water Transfer", waterTransferID, out var actionResult))
            {
                return actionResult;
            }

            var currentUser = UserContext.GetUserFromHttpContext(_dbContext, HttpContext);

            var validationMessages = WaterTransfer.ValidateCancelTransfer(waterTransferRegistrationDto, waterTransferDto, currentUser);
            validationMessages.ForEach(vm => { ModelState.AddModelError(vm.Type, vm.Message); });

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            waterTransferDto = WaterTransfer.ChangeWaterRegistrationStatus(_dbContext, waterTransferID, waterTransferRegistrationDto,
                WaterTransferRegistrationStatusEnum.Canceled);
            var smtpClient = HttpContext.RequestServices.GetRequiredService<SitkaSmtpClientService>();
            var mailMessages = GenerateCancelTransferEmail(_rioConfiguration.WEB_URL, waterTransferDto, smtpClient);
            foreach (var mailMessage in mailMessages)
            {
                SendEmailMessage(smtpClient, mailMessage);
            }

            return Ok(waterTransferDto);
        }



        [HttpGet("water-transfers/{waterTransferID}/parcels/{userID}")]
        [OfferManageFeature]
        public ActionResult<List<WaterTransferRegistrationParcelDto>> GetParcelsForWaterTransferID([FromRoute] int waterTransferID, [FromRoute] int userID)
        {
            var waterTransferRegistrationParcelDtos = WaterTransferRegistrationParcel.ListByWaterTransferIDAndAccountID(_dbContext, waterTransferID, userID);
            if (waterTransferRegistrationParcelDtos == null)
            {
                return NotFound();
            }

            return Ok(waterTransferRegistrationParcelDtos);
        }

        [HttpPost("water-transfers/{waterTransferID}/selectParcels")]
        [OfferManageFeature]
        public ActionResult<List<WaterTransferDto>> SelectParcels([FromRoute] int waterTransferID, [FromBody] WaterTransferRegistrationDto waterTransferRegistrationDto)
        {
            if (!_rioConfiguration.ALLOW_TRADING)
            {
                return BadRequest();
            }

            var waterTransferDto = WaterTransfer.GetByWaterTransferID(_dbContext, waterTransferID);
            if (ThrowNotFound(waterTransferDto, "Water Transfer", waterTransferID, out var actionResult))
            {
                return actionResult;
            }
            var validationMessages = WaterTransferRegistrationParcel.ValidateParcels(waterTransferRegistrationDto.WaterTransferRegistrationParcels, waterTransferDto);
            validationMessages.ForEach(vm => {ModelState.AddModelError(vm.Type, vm.Message);});

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var waterTransferRegistrationParcelDtos = WaterTransferRegistrationParcel.SaveParcels(_dbContext, waterTransferID, waterTransferRegistrationDto);
            return Ok(waterTransferRegistrationParcelDtos);
        }

        private void SendEmailMessage(SitkaSmtpClientService smtpClient, MailMessage mailMessage)
        {
            mailMessage.IsBodyHtml = true;
            mailMessage.From = smtpClient.GetDefaultEmailFrom();
            mailMessage.ReplyToList.Add(_rioConfiguration.LeadOrganizationEmail);
            SitkaSmtpClientService.AddBccRecipientsToEmail(mailMessage, EFModels.Entities.User.GetEmailAddressesForAdminsThatReceiveSupportEmails(_dbContext));
            smtpClient.Send(mailMessage);
        }

        private static List<MailMessage> GenerateConfirmTransferEmail(string rioUrl, WaterTransferDto waterTransfer,
            SitkaSmtpClientService smtpClient)
        {
            var receivingAccount = waterTransfer.BuyerRegistration.Account;
            var transferringAccount = waterTransfer.SellerRegistration.Account;
            var mailMessages = new List<MailMessage>();
            var stringToReplace = "REPLACE_WITH_ACCOUNT_NUMBER";
            var messageBody = $@"The buyer and seller have both registered this transaction, and your annual water allocation has been updated.
<ul>
    <li><strong>Buyer:</strong> {receivingAccount.AccountName} ({string.Join(", ", receivingAccount.Users.Select(x=>x.Email))})</li>
    <li><strong>Seller:</strong> {transferringAccount.AccountName} ({string.Join(", ", transferringAccount.Users.Select(x => x.Email))})</li>
    <li><strong>Quantity:</strong> {waterTransfer.AcreFeetTransferred} acre-feet</li>
</ul>
<a href=""{rioUrl}/landowner-dashboard/{stringToReplace}"">View your Landowner Dashboard</a> to see your current water allocation, which has been updated to reflect this trade.
<br /><br />
{smtpClient.GetDefaultEmailSignature()}";
            var mailTos = new List<AccountDto> { receivingAccount, transferringAccount }.SelectMany(x=>x.Users);
            foreach (var mailTo in mailTos)
            {
                var specificMessageBody = messageBody.Replace(stringToReplace,
                    (receivingAccount.Users.Contains(mailTo)
                        ? receivingAccount.AccountNumber
                        : transferringAccount.AccountNumber).ToString());
                var mailMessage = new MailMessage
                {
                    Subject = "Water Transfer Registered",
                    Body = $"Hello {mailTo.FullName},<br /><br />{specificMessageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }
        private static List<MailMessage> GenerateCancelTransferEmail(string rioUrl, WaterTransferDto waterTransfer,
            SitkaSmtpClientService smtpClient)
        {
            var receivingAccount = waterTransfer.BuyerRegistration.Account;
            var transferringAccount = waterTransfer.SellerRegistration.Account;
            var mailMessages = new List<MailMessage>();
            var stringToReplace = "REPLACE_WITH_ACCOUNT_NUMBER";
            var messageBody = $@"This transaction has been canceled, and your annual water allocation will not be updated.
<ul>
    <li><strong>Buyer:</strong> {receivingAccount.AccountName} ({string.Join(", ", receivingAccount.Users.Select(x => x.Email))})</li>
    <li><strong>Seller:</strong> {transferringAccount.AccountName} ({string.Join(", ", transferringAccount.Users.Select(x => x.Email))})</li>
    <li><strong>Quantity:</strong> {waterTransfer.AcreFeetTransferred} acre-feet</li>
</ul>
<a href=""{rioUrl}/landowner-dashboard/{stringToReplace}"">View your Landowner Dashboard</a> to see your current water allocation, which has not been updated to reflect this trade.
<br /><br />
{smtpClient.GetDefaultEmailSignature()}";
            var mailTos = new List<AccountDto> { receivingAccount, transferringAccount }.SelectMany(x=>x.Users);
            foreach (var mailTo in mailTos)
            {
                var specificMessageBody = messageBody.Replace(stringToReplace,
                    (receivingAccount.Users.Contains(mailTo)
                        ? receivingAccount.AccountNumber
                        : transferringAccount.AccountNumber).ToString());
                var mailMessage = new MailMessage
                {
                    Subject = "Water Transfer Canceled",
                    Body = $"Hello {mailTo.FullName},<br /><br />{specificMessageBody}"
                };
                mailMessage.To.Add(new MailAddress(mailTo.Email, mailTo.FullName));
                mailMessages.Add(mailMessage);
            }
            return mailMessages;
        }
    }
}