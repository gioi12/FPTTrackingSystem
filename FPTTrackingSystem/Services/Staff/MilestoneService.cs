using Azure;
using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using Repositories.Staff;
using System.IO.Pipelines;
using Mapster;

namespace FPTTrackingSystem.Services.Staff
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMilestoneRepository _milestoneRepository;
        private readonly AuthUtils _authUtils;

        public MilestoneService(IGroupRepository groupRepository, IMilestoneRepository milestoneRepository, AuthUtils authUtils)
        {
            _authUtils = authUtils;
            _groupRepository = groupRepository;
            _milestoneRepository = milestoneRepository;
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> CreateMilestoneInSemester(List<MilestoneCreateRequest> request)
        {
            var user = await _authUtils.GetUserInfoFromCookie();

            var milestones = request.Select(x => new Milestone
            {
                Name = x.Name,
                Description = x.Description,
                MajorId = x.MajorId,
                SemesterId = x.SemesterId,
                CreateAt = DateTime.Now,
                CreateBy = user.Id,
                Deadline = x.Deadline
            }).ToList();

            var list = await _milestoneRepository.NewMilestontes(milestones);
            var response = list.Adapt<List<MilestoneResponse>>();

            return ApiResponse<List<MilestoneResponse>>.Success(response);
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> DeleteMilestone(int id)
        {
            var list = await _milestoneRepository.deleteMilestone(id);
            return ApiResponse<List<MilestoneResponse>>.Success(list.Adapt<List<MilestoneResponse>>());
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> GetMilestoneByMajorAndSemester(int majorId, int semesterId)
        {
            var list = await _milestoneRepository.GetByMajorAndSemester(majorId, semesterId);
            var response = list.Adapt<List<MilestoneResponse>>();
            return ApiResponse<List<MilestoneResponse>>.Success(response);
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> UpdateInfoMilestone(List<MilestoneCreateRequest> request)
        {
            int majorId = request.First().MajorId;
            int semesterId = request.First().SemesterId;
            var list = await _milestoneRepository.GetByMajorAndSemester(majorId, semesterId);
            foreach (var milestone in list)
            {
                var requestItem = request.FirstOrDefault(x => x.Id == milestone.Id);
                if (requestItem != null)
                {
                    milestone.Name = requestItem.Name;
                    milestone.Description = requestItem.Description;
                    milestone.Deadline = requestItem.Deadline;
                }
            }
            var listUpadated = await _milestoneRepository.updateMilestontes(list);
            var response = listUpadated.Adapt<List<MilestoneResponse>>();
            return ApiResponse<List<MilestoneResponse>>.Success(response);
        }
    }
}
