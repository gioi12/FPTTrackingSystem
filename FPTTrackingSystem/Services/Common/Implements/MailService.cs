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

        public async Task SendAnnounceMail(MailRequest request)
        {
            await _rabbitMQProducer.SendMessage(new List<MailRequest> { request});
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
                    foreach (var to in mail.To)
                    {
                        email.To.Add(MailboxAddress.Parse(to));
                    }
                    email.Subject = mail.Subject;
                    email.Body = new BodyBuilder { HtmlBody = mail.Body }.ToMessageBody();

                    if (mail.Cc != null && mail.Cc.Any())
                    {
                        foreach (var cc in mail.Cc)
                        {
                            email.Cc.Add(MailboxAddress.Parse(cc));
                        }
                    }
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
