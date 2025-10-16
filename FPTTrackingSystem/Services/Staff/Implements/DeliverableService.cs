using DataTranferObjects.Staff.Response;
using FPTTrackingSystem.Services.Staff.Interfaces;
using FPTTrackingSystem.Utilities;
using Mapster;
using Microsoft.IdentityModel.Tokens;
using Repositories.Staff.Interfaces;

namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class DeliverableService : IDeliverableSevice
    {
        private readonly IDeliverableRepository _deliverableRepository;
        private readonly ISemesterRepository _semesterRepository;
        
        public DeliverableService(IDeliverableRepository deliverableRepository, ISemesterRepository semesterRepository)
        {
            _deliverableRepository = deliverableRepository;
            _semesterRepository = semesterRepository;
        }
        public async Task<List<DeliverableRes>> GetDeliverableByCodeAndSemester(int semesterId, int code)
        {
            var list = await _deliverableRepository.GetByCodeAndSemester(code, semesterId);
            var semester = await _semesterRepository.GetSemesterByIdAsync(semesterId);
            var res = list.Adapt<List<DeliverableRes>>();
            foreach (var item in res)
            {
                item.StartAt = semester.StartAt;
                item.EndAt = item.Deadline != null ? DateTimeUtils.GetDeadlineDate(item.Deadline, (List<Entities.Models.SemesterWeek>)semester.SemesterWeeks) : null;
            }
            return res;
        }
    }
}
