using DataTranferObjects.Common.Request;
using DataTranferObjects.Common.Response;
using Entities.Models;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Common.MQ;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Repositories.Common.Interfaces;

namespace FPTTrackingSystem.Services.Common.Implements
{
    public class MailService : IMailService
    {
        private readonly RabbitMQProducer _rabbitMQProducer;
        private readonly IMailSettingCache _mailCache;
        private readonly IMailRepository _mailRepository;
        private MailSettings _settings => _mailCache.Settings;

        public MailService(IMailSettingCache mailCache, RabbitMQProducer rabbitMQProducer,IMailRepository mailRepository) { 
            _rabbitMQProducer = rabbitMQProducer;
            _mailCache= mailCache;
            _mailRepository = mailRepository;
        }

        public async System.Threading.Tasks.Task SendAnnounceMail(MailRequest request)
        {
            await _rabbitMQProducer.SendMessage(new List<MailRequest> { request});
        }

        public async System.Threading.Tasks.Task SendEmailAsync(List<MailRequest> request)
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
                            await System.Threading.Tasks.Task.Delay(2000); 
                        }
                    }
                }
            }

            await smtp.DisconnectAsync(true);
        }

        public MailSettingsRes GetMailSettings()
        {
            return new MailSettingsRes
            {
                Mail = _settings.Mail,
                DisplayName = _settings.DisplayName,
                Host = _settings.Host,
                Port = _settings.Port
            };
        }

        public async Task<MailSettingsRes> NewMailSettingsAsync(MailSettings request)
        {
            var mail = new MailSetting
            {
                Mail = request.Mail,
                DisplayName = request.DisplayName,
                Password = request.Password,
                Host = request.Host,
                Port = request.Port,
                IsActive = true
            };

            await _mailRepository.NewMailSetting(mail);

            await _mailCache.ReloadAsync();

            return new MailSettingsRes
            {
                Mail = mail.Mail,
                DisplayName = mail.DisplayName,
                Host = mail.Host,
                Port = (int)mail.Port
            };
        }
    }
}
