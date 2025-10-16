using DataTranferObjects.Staff.Response;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface IDeliverableSevice
    {
        Task<List<DeliverableRes>> GetDeliverableByCodeAndSemester(int semesterId,int code);
    }
}
