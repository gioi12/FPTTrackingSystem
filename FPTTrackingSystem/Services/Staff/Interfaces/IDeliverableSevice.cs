using DataTranferObjects.Common.Request;
using DataTranferObjects.Staff.Response;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IDeliverableSevice
    {
        Task<List<DeliverableRes>> GetDeliverableByCodeAndSemester(int semesterId,int code);

        Task<string> UploadFileMilestone(IFormFile file, int groupId, int deliverableId);

    }
}
