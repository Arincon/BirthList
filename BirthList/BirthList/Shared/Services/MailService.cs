using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BirthList.Shared.Services
{
    public class MailService : IMailService
    {
        private IConfiguration _configuration;
        private string _apikey;

        public MailService(IConfiguration config)
        {
            _configuration = config;
            _apikey = _configuration["SendGridKey"];
        }


        public async Task<bool> SendEmail(PurchasedPresent presentInfo)
        {
            return await ExecuteSendEmail(presentInfo);
        }

        public async Task<bool> ExecuteSendEmail(PurchasedPresent presentInfo)
        {
            var client = new SendGridClient(_apikey);
            var from = new EmailAddress("adrl1988@gmail.com");
            var subject = $"{presentInfo.Title} bought ({presentInfo.NewlyBought})";
            var arinconMail = new EmailAddress("adrl1988@gmail.com");
            var paupepaMail = new EmailAddress("paulapp.28@gmail.com");
            var plainTextContent = $"{presentInfo.Title}";
            var htmlContent = $"<strong>Someone just bought something!</strong><div>{presentInfo.Title}</div>";
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, new List<EmailAddress> { arinconMail, paupepaMail }, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response.IsSuccessStatusCode;
        }
    }
}
