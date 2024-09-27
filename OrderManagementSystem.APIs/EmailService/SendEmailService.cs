
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using OrderManagementSystem.Core.Entities;
using System.Net.Mail;
using System.Net;
using Org.BouncyCastle.Cms;
using static MailKit.Telemetry;

namespace OrderManagementSystem.APIs.EmailService
{
    public class SendEmailService : IEmailService
    {
        private readonly IConfiguration _conf;

        public SendEmailService( IConfiguration configuration)
        {
            _conf = configuration;
        }

        public async Task<bool> SendEmailAsync(string from, string recipients, string subject, string body)
        {
            try
            {
                var senderEmail = _conf["EmailSettings:senderEmail"];
                var senderPassword = _conf["EmailSettings:senderPassword"];

                var emailMessage = new MailMessage();
                emailMessage.From = new MailAddress(from);
                emailMessage.To.Add(recipients);
                emailMessage.Subject = subject;
                emailMessage.Body = body;

                var smtpClient = new System.Net.Mail.SmtpClient(_conf["EmailSettings:SmtpClientServer"], int.Parse(_conf["EmailSettings:SmtpClientPort"]))
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                Console.WriteLine("Sending email...");
                await smtpClient.SendMailAsync(emailMessage);
                Console.WriteLine("Email sent successfully.");
                return true;
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP error: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return false;
            }
        }
    }
}
