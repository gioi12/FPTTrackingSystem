using DataTranferObjects.Common.Request;

namespace FPTTrackingSystem.Services.Common.Interfaces
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest request);
    }
}
