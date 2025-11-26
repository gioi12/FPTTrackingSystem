using DataTranferObjects.Common.Request;
using DataTranferObjects.Common.Response;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(List<MailRequest> request);

        Task SendAnnounceMail(MailRequest request);
        MailSettingsRes GetMailSettings();
        Task<MailSettingsRes> NewMailSettingsAsync(MailSettings request);

    }
}
