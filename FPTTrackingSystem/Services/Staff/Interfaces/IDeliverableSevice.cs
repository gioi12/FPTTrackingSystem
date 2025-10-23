using DataTranferObjects.Common.Request;
using DataTranferObjects.Staff.Response;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IDeliverableSevice
    {
        Task<List<DeliverableRes>> GetDeliverableByCodeAndSemester(int semesterId,int code);

        Task<string> UploadFileMilestoneItem(IFormFile file, int groupId, int deliveryItemId);
        Task<List<GroupDeliverableRes>> GetDeliverableByGroupId(int groupId);
        Task<DeliverableDetailRes> GetDeliverableByIdAndGroupId(int groupId, int deliverableId);
        Task<string> ConfirmDeliverable(int groupId, int deliverableId);
        Task<List<DeliverableGroupDetailDTO>> GetDeliverableGroupsByGroupIdAsync(int groupId);
    }
}
