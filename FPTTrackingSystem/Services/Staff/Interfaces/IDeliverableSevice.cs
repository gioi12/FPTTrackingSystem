using DataTranferObjects.Common.Request;
using DataTranferObjects.Staff.Response;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IDeliverableSevice
    {
        Task<List<DeliverableRes>> GetDeliverableByCodeAndSemester(int semesterId,int code);

        Task<string> UploadFileMilestoneItem(IFormFile file, int groupId, int deliveryItemId, string semester);
        Task<List<GroupDeliverableRes>> GetDeliverableByGroupId(int groupId);
        Task<DeliverableDetailRes> GetDeliverableByIdAndGroupId(int groupId, int deliverableId);
        Task<string> ConfirmDeliverable(int groupId, int deliverableId, string? note);
        Task<List<DeliverableGroupDetailDTO>> GetDeliverableGroupsByGroupIdAsync(int groupId);
        Task DeleteFileMilestoneItem(int attachmentId);
        Task MarkDownload(int attachmentId);
        Task<string> RejectDeliverable(int groupId, int deliverableId, string? note);

    }
}
