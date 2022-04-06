using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Rio.Models.DataTransferObjects.User;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Rio.API.Services
{
    public class SitkaSmtpClientService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly RioConfiguration _rioConfiguration;

        //private static readonly ILog _logger = LogManager.GetLogger(typeof(SitkaSmtpClient));

        public SitkaSmtpClientService(ISendGridClient sendGridClient, IOptions<RioConfiguration> rioConfiguration)
        {
            _sendGridClient = sendGridClient;
            _rioConfiguration = rioConfiguration.Value;
        }

        /// <summary>
        /// Sends an email including mock mode and address redirection  <see cref="RioConfiguration.SITKA_EMAIL_REDIRECT"/>, then calls onward to <see cref="SendDirectly"/>
        /// </summary>
        /// <param name="message"></param>
        public async Task Send(MailMessage message)
        {
            var messageWithAnyAlterations = AlterMessageIfInRedirectMode(message);
            var messageAfterAlterationsAndCreatingAlternateViews = CreateAlternateViewsIfNeeded(messageWithAnyAlterations);
            await SendDirectly(messageAfterAlterationsAndCreatingAlternateViews);
        }

        private static MailMessage CreateAlternateViewsIfNeeded(MailMessage message)
        {
            if (!message.IsBodyHtml)
            {
                return message;
            }
            // Define the plain text alternate view and add to message
            const string plainTextBody = "You must use an email client that supports HTML messages";

            var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, MediaTypeNames.Text.Plain);

            message.AlternateViews.Add(plainTextView);

            // Define the html alternate view with embedded image and
            // add to message. To reference images attached as linked
            // resources from your HTML message body, use "cid:contentID"
            // in the <img> tag...
            var htmlBody = message.Body;

            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            message.AlternateViews.Add(htmlView);


            return message;
        }


        /// <summary>
        /// Sends an email message at a lower level than <see cref="Send"/>, skipping mock mode and address redirection  <see cref="RioConfiguration.SITKA_EMAIL_REDIRECT"/>
        /// </summary>
        /// <param name="mailMessage"></param>
        public async Task SendDirectly(MailMessage mailMessage)
        {
            //if (!string.IsNullOrWhiteSpace(RioConfiguration.MailLogBcc))
            //{
            //    sendGridMessage.Bcc.Add(SitkaWebConfiguration.MailLogBcc);
            //}
            var defaultEmailFrom = GetDefaultEmailFrom();
            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(defaultEmailFrom.Address, defaultEmailFrom.DisplayName),
                Subject = mailMessage.Subject,
                PlainTextContent = mailMessage.Body,
                HtmlContent = mailMessage.IsBodyHtml ? mailMessage.Body : null
            };
            sendGridMessage.AddTos(mailMessage.To.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList());
            if (mailMessage.CC.Any())
            {
                sendGridMessage.AddCcs(mailMessage.CC.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList());
            }

            if (mailMessage.Bcc.Any())
            {
                sendGridMessage.AddBccs(mailMessage.Bcc.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList());
            }

            var response = await _sendGridClient.SendEmailAsync(sendGridMessage);
            //_logger.Info($"Email sent to SMTP server \"{smtpClient.Host}\", Details:\r\n{humanReadableDisplayOfMessage}");
        }

        /// <summary>
        /// Alter message TO, CC, BCC if the setting <see cref="RioConfiguration.SITKA_EMAIL_REDIRECT"/> is set
        /// Appends the real to the body
        /// </summary>
        /// <param name="realMailMessage"></param>
        /// <returns></returns>
        private MailMessage AlterMessageIfInRedirectMode(MailMessage realMailMessage)
        {
            var redirectEmail = _rioConfiguration.SITKA_EMAIL_REDIRECT;
            var isInRedirectMode = !String.IsNullOrWhiteSpace(redirectEmail);

            if (!isInRedirectMode)
            {
                return realMailMessage;
            }

            ClearOriginalAddressesAndAppendToBody(realMailMessage, "To", realMailMessage.To);
            ClearOriginalAddressesAndAppendToBody(realMailMessage, "CC", realMailMessage.CC);
            ClearOriginalAddressesAndAppendToBody(realMailMessage, "BCC", realMailMessage.Bcc);

            realMailMessage.To.Add(redirectEmail);

            return realMailMessage;
        }

        private static void ClearOriginalAddressesAndAppendToBody(MailMessage realMailMessage, string addressType, ICollection<MailAddress> addresses)
        {
            var newline = realMailMessage.IsBodyHtml ? "<br />" : Environment.NewLine;
            var separator = newline + "\t";

            var toExpected = addresses.Aggregate(String.Empty, (s, mailAddress) => s + Environment.NewLine + "\t" + mailAddress.ToString());
            if (!String.IsNullOrWhiteSpace(toExpected))
            {
                var toAppend =
                    $"{newline}{separator}Actual {addressType}:{(realMailMessage.IsBodyHtml ? toExpected.HtmlEncodeWithBreaks() : toExpected)}";
                realMailMessage.Body += toAppend;

                for (var i = 0; i < realMailMessage.AlternateViews.Count; i++)
                {
                    var stream = realMailMessage.AlternateViews[i].ContentStream;
                    using (var reader = new StreamReader(stream))
                    {
                        var alternateBody = reader.ReadToEnd();
                        alternateBody += toAppend;
                        var newAlternateView = AlternateView.CreateAlternateViewFromString(alternateBody, null, realMailMessage.AlternateViews[i].ContentType.MediaType);
                        realMailMessage.AlternateViews[i].LinkedResources.ToList().ForEach(x => newAlternateView.LinkedResources.Add(x));
                        realMailMessage.AlternateViews[i] = newAlternateView;
                    }
                }


            }
            addresses.Clear();
        }

        private static string FlattenMailAddresses(IEnumerable<MailAddress> addresses)
        {
            return String.Join("; ", addresses.Select(x => x.ToString()));
        }

        public string GetDefaultEmailSignature()
        {
            string defaultEmailSignature = $@"<br /><br />
Respectfully, the {_rioConfiguration.PlatformLongName} team
<br /><br />
***
<br /><br />
You have received this email because you are a registered user of the {_rioConfiguration.PlatformLongName}. 
<br /><br />
Contact <a href=""mailto:{_rioConfiguration.LeadOrganizationEmail}"">{_rioConfiguration.LeadOrganizationEmail}</a> for more information";
            return defaultEmailSignature;
        }

        public string GetSupportNotificationEmailSignature()
        {
            string supportNotificationEmailSignature = $@"<br /><br />
Respectfully, the {_rioConfiguration.PlatformLongName} team
<br /><br />
***
<br /><br />
You have received this email because you are assigned to receive support notifications within the {_rioConfiguration.PlatformLongName}. 
<br /><br />
<a href=""mailto:{_rioConfiguration.LeadOrganizationEmail}"">{_rioConfiguration.LeadOrganizationEmail}</a>";
            return supportNotificationEmailSignature;
        }

        public MailAddress GetDefaultEmailFrom()
        {
            return new MailAddress("donotreply@sitkatech.net", $"{_rioConfiguration.PlatformLongName}");
        }

        public static void AddBccRecipientsToEmail(MailMessage mailMessage, IEnumerable<string> recipients)
        {
            foreach (var recipient in recipients)
            {
                mailMessage.Bcc.Add(recipient);
            }
        }

        public static void AddCcRecipientsToEmail(MailMessage mailMessage, IEnumerable<string> recipients)
        {
            foreach (var recipient in recipients)
            {
                mailMessage.CC.Add(recipient);
            }
        }
    }
}