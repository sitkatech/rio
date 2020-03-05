using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using Rio.Models.DataTransferObjects.User;

namespace Rio.API.Services
{
    public class SitkaSmtpClientService
    {
        private readonly RioConfiguration _rioConfiguration;

        //private static readonly ILog _logger = LogManager.GetLogger(typeof(SitkaSmtpClient));

        public SitkaSmtpClientService(RioConfiguration rioConfiguration)
        {
            _rioConfiguration = rioConfiguration;
        }

        /// <summary>
        /// Sends an email including mock mode and address redirection  <see cref="RioConfiguration.SITKA_EMAIL_REDIRECT"/>, then calls onward to <see cref="SendDirectly"/>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="linkedResources"></param>
        public void Send(MailMessage message, IEnumerable<LinkedResource> linkedResources = null)
        {
            var messageWithAnyAlterations = AlterMessageIfInRedirectMode(message);
            var messageAfterAlterationsAndCreatingAlternateViews = CreateAlternateViewsIfNeeded(messageWithAnyAlterations, linkedResources);
            SendDirectly(messageAfterAlterationsAndCreatingAlternateViews);
        }

        private static MailMessage CreateAlternateViewsIfNeeded(MailMessage message, IEnumerable<LinkedResource> linkedResources)
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

            if (linkedResources != null)
            {
                foreach (var linkedResource in linkedResources)
                {
                    htmlView.LinkedResources.Add(linkedResource);
                }
            }
            message.AlternateViews.Add(htmlView);


            return message;
        }


        /// <summary>
        /// Sends an email message at a lower level than <see cref="Send"/>, skipping mock mode and address redirection  <see cref="RioConfiguration.SITKA_EMAIL_REDIRECT"/>
        /// </summary>
        /// <param name="mailMessage"></param>
        public void SendDirectly(MailMessage mailMessage)
        {
            //if (!string.IsNullOrWhiteSpace(RioConfiguration.MailLogBcc))
            //{
            //    mailMessage.Bcc.Add(SitkaWebConfiguration.MailLogBcc);
            //}
            var humanReadableDisplayOfMessage = GetHumanReadableDisplayOfMessage(mailMessage);
            var smtpClient = new SmtpClient(_rioConfiguration.SMTP_HOST, _rioConfiguration.SMTP_PORT);
            smtpClient.Send(mailMessage);
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

        private static string GetHumanReadableDisplayOfMessage(MailMessage mm)
        {
            var currentDateFormattedForEmail = DateTime.Now.ToString("ddd dd MMM yyyy HH:mm:ss zzz");
            var messageString = $@"Date: {currentDateFormattedForEmail}
From: {mm.From}
To: {FlattenMailAddresses(mm.To)}
Reply-To: {FlattenMailAddresses(mm.ReplyToList)}
CC: {FlattenMailAddresses(mm.CC)}
Bcc: {FlattenMailAddresses(mm.Bcc)}
Subject: {mm.Subject}

{mm.Body}";

            return messageString;
        }

        private static string FlattenMailAddresses(IEnumerable<MailAddress> addresses)
        {
            return String.Join("; ", addresses.Select(x => x.ToString()));
        }

        public static void AddReplyToEmail(MailMessage mailMessage)
        {
            mailMessage.ReplyToList.Add("admin@rrbwsd.com");
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
<a href=""mailto:{_rioConfiguration.LeadOrganizationEmail}"">{_rioConfiguration.LeadOrganizationEmail}</a>";
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
            return new MailAddress("donotreply @sitkatech.com", $"{_rioConfiguration.PlatformLongName}");
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