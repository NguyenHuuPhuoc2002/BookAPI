using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit.Text;
using MimeKit;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;
using MailKit.Net.Smtp;
using BookAPI.Models;

namespace Service
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(MailRequest request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailSetting:EmailUsername").Value));
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = request.Subject;
            email.Body = new TextPart(TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_configuration.GetSection("EmailSetting:EmailHost").Value, 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration.GetSection("EmailSetting:EmailUsername").Value, _configuration.GetSection("EmailSetting:EmailPassword").Value);
            smtp.Send(email);
            smtp.Disconnect(true);

            return await Task.FromResult(true);

        }
    }
}
