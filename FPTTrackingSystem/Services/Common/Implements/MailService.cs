using DataTranferObjects.Common.Request;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Common.MQ;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
namespace FPTTrackingSystem.Services.Common.Implements
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly RabbitMQProducer _rabbitMQProducer;
        public MailService(IOptions<MailSettings> settings,RabbitMQProducer rabbitMQProducer) { 
            _settings = settings.Value;
            _rabbitMQProducer = rabbitMQProducer;
        }

        public async Task SendAnnounceMail(MailAnnounceRequest request)
        {
            List<MailRequest> mailRequests = new List<MailRequest>();
            foreach (var mail in request.To)
            {
                mailRequests.Add(new MailRequest
                {
                    To = mail,
                    Subject = request.Subject,
                    Body = request.Body
                });
            }
            await _rabbitMQProducer.SendMessage(mailRequests);
        }

        public async Task SendEmailAsync(List<MailRequest> request)
        {
            if (request == null || !request.Any())
                return;

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);

            foreach (var mail in request)
            {
                MimeMessage email;
                try
                {
                    email = new MimeMessage();
                    email.Sender = new MailboxAddress(_settings.DisplayName, _settings.Mail);
                    email.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Mail));
                    email.To.Add(MailboxAddress.Parse(mail.To));
                    email.Subject = mail.Subject;
                    email.Body = new BodyBuilder { HtmlBody = mail.Body }.ToMessageBody();
                }
                catch (Exception ex)
                {
                    continue; 
                }
                int retryCount = 0;
                bool sent = false;
                while (!sent && retryCount < 2)
                {
                    try
                    {
                        await smtp.SendAsync(email);
                        sent = true;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                        if (retryCount < 2)
                        {
                            await Task.Delay(2000); 
                        }
                    }
                }
            }

            await smtp.DisconnectAsync(true);
        }

    }
}
