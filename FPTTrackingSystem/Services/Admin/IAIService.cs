using DataTranferObjects.Common.Request;
using DataTranferObjects.Common.Response;

namespace FPTTrackingSystem.Services.Admin
{
    public interface IAIService
    {
        Task<AISettingsRes> GetAISettings();
        Task<AISettingsRes> NewAISettings(NewAISettingsReq setting);
        Task<string> AskAsync(string prompt,int? groupId);

    }
}
