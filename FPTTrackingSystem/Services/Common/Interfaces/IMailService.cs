using DataTranferObjects.Common.Request;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(List<MailRequest> request);

        Task SendAnnounceMail(MailRequest request);
    }
}
