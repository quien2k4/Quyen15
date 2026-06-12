using System.Net;
using System.Net.Mail;

namespace Quyen15.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var host = _configuration["Smtp:Host"]?.Trim();
            var portText = _configuration["Smtp:Port"]?.Trim() ?? "587";
            var userName = _configuration["Smtp:UserName"]?.Trim();
            var password = _configuration["Smtp:Password"]?.Replace(" ", "").Trim();
            var fromEmail = _configuration["Smtp:FromEmail"]?.Trim();
            var fromName = _configuration["Smtp:FromName"]?.Trim();

            int port = int.Parse(portText);

            if (string.IsNullOrWhiteSpace(host) ||
                string.IsNullOrWhiteSpace(userName) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(fromEmail))
            {
                throw new Exception("SMTP configuration is missing.");
            }

            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail.Trim());

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(userName, password),
                EnableSsl = true
            };

            await client.SendMailAsync(message);
        }
    }
}