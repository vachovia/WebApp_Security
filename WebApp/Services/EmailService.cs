using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using WebApp.Settings;
using WebApp.Services.Interfaces;

namespace WebApp.Services
{
    public class EmailService : IEmailService
    {
        IOptions<SmtpSettings> _smtpSettings { get; set; }
        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        public async Task SendAsync(string to, string subject, string body)
        {
            var message = new MailMessage(_smtpSettings.Value.From, to, subject, body);

            using (var emailClient = new SmtpClient(_smtpSettings.Value.Host, _smtpSettings.Value.Port))
            {
                emailClient.Credentials = new NetworkCredential(_smtpSettings.Value.User, _smtpSettings.Value.Password);

                await emailClient.SendMailAsync(message);
            }
        }
    }
}
