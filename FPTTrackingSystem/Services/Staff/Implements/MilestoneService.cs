using Azure;
using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using FPTTrackingSystem.Utilities;
using FPTTrackingSystem.Wrappers;
using System.IO.Pipelines;
using Mapster;
using FPTTrackingSystem.Services.Staff.Interfaces;
using System.ComponentModel.DataAnnotations;
using Repositories.Staff.Interfaces;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Common.Implements;
using DataTranferObjects.Enum;
using Repositories.Staff.Implements;

namespace FPTTrackingSystem.Services.Staff.Implementations
{
    public class MilestoneService : IMilestoneService
    {
        private readonly IMilestoneRepository _milestoneRepository;
        private readonly IDeliverableRepository _deliverableRepository;
        private readonly ISemesterRepository _semesterRepository;
        private readonly ILogService _logService;
        private readonly AuthUtils _authUtils;
        public MilestoneService(IGroupRepository groupRepository, IMilestoneRepository milestoneRepository, AuthUtils authUtils,ILogService logService, IDeliverableRepository deliverableRepository,ISemesterRepository semesterRepository)
        {
            _authUtils = authUtils;
            _milestoneRepository = milestoneRepository;
            _logService = logService;
            _deliverableRepository = deliverableRepository;
            _semesterRepository = semesterRepository;
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> CreateMilestoneInSemester(List<MilestoneCreateRequest> request)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            int majorCateId = request.FirstOrDefault().MajorCateId;
            var semester = await _semesterRepository.findActive();
            var milestones = request.Select(x => new Milestone
            {
                Name = x.Name,
                Description = x.Description,
                MajorId = x.MajorCateId,
                CreateAt = DateTime.Now,
                CreateBy = user.Id,
                IsActive = true,
                Deliverables = semester != null ? new List<Deliverable>
                {
                    new Deliverable
                    {
                        Name = x.Name,
                        Description = x.Description,
                        SemesterId = semester.Id,
                        IsActive = true,
                        MajorId = x.MajorCateId
                    }
                } : new List<Deliverable>()
            }).ToList();
    
            var list = await _milestoneRepository.NewMilestontes(milestones, majorCateId);
            var logs = milestones.Select(x => new Log
            {
                Name = "Thêm milestone " + x.Name,
                EntityName = "Milestone",
                EntityId = x.Id,
                Action = StringEnum.Create,
                Description = x.Description,
                UserId = (int)user.Id,
                CreateAt = DateTime.Now
            }).ToList();
            var delilogs = milestones
                .SelectMany(m => m.Deliverables.Select(d => new Log
                {
                    Name = "Thêm Deliverable " + d.Name,
                    EntityName = "Deliverable",
                    EntityId = d.Id,
                    Action = StringEnum.Create,
                    Description = d.Description,
                    UserId = (int)user.Id,
                    CreateAt = DateTime.Now
                }))
                .ToList();
            // gop 2 list
            logs.AddRange(delilogs);
            await _logService.AddRangeLogAsync(logs);
            var response = list.Adapt<List<MilestoneResponse>>();

            return ApiResponse<List<MilestoneResponse>>.Success(response);
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> DeleteMilestone(int id)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var mile = await _milestoneRepository.GetMilestone(id);
            var deli = mile.Deliverables.FirstOrDefault();
            var list = await _milestoneRepository.DeleteMilestone(id);
            Log log = new Log
            {
                Name = "Xoá milestone " + mile.Name,
                EntityName = "Milestone",
                EntityId = mile.Id,
                Action = StringEnum.Delete,
                Description = mile.Description,
                UserId = (int)user.Id,
                CreateAt = DateTime.Now
            };
            Log deliLog = new Log
            {
                Name = "Xoá Deliverable " + deli.Name,
                EntityName = "Deliverable",
                EntityId = deli.Id,
                Action = StringEnum.Delete,
                Description = deli.Description,
                UserId = (int)user.Id,
                CreateAt = DateTime.Now
            };
            await _logService.AddRangeLogAsync(new List<Log>
            {
                log, deliLog
            });
            return ApiResponse<List<MilestoneResponse>>.Success(list.Adapt<List<MilestoneResponse>>());
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> GetMilestonesByMajor(int majorId)
        {
            var list = await _milestoneRepository.GetByMajor(majorId);
            var response = list.Adapt<List<MilestoneResponse>>();
            return ApiResponse<List<MilestoneResponse>>.Success(response);
        }

        public async Task<ApiResponse<List<MilestoneResponse>>> UpdateInfoMilestone(MilestoneUpdateRequest request)
        {
            var user = await _authUtils.GetUserInfoFromCookie();
            var mile = await _milestoneRepository.GetMilestone(request.Id);
            int majorId = request.MajorCateId;

            if (mile == null) throw new ValidationException("Not found milestone");
            //lay ra item them
            var addItems = request.items?.Where(x=> x.id == null).Select(x=> new MilestoneItem
            {
                Name = x.name,
                Description = x.description,
                CreateAt = DateTime.Now,
                CreateBy = user.Id,
                MilestoneId = mile.Id, 
            }).ToList() ?? new List<MilestoneItem>();
            //lay ra item can update
            var updateItems = request.items?.Where(x => x.id != null).ToList() ?? new List<MilestoneItemRequest?>();
            // lay ra item can delete
            var deleteItems = mile.MilestoneItems.Where(x => !updateItems.Any(y => y.id == x.Id)).ToList();
            mile.Name = request.Name;
            mile.Description = request.Description;
            mile.Deadline = request.Deadline;
            //them
            addItems.ForEach(x => mile.MilestoneItems.Add(x));
            //cap nhat
            foreach (var updated in updateItems)
            {
                var existing = mile.MilestoneItems.FirstOrDefault(x => x.Id == updated.id);
                if (existing != null)
                {
                    existing.Name = updated.name;
                    existing.Description = updated.description;
                }
            }
            // xoa
            deleteItems.ForEach(x => mile.MilestoneItems.Remove(x));
            var list = await _milestoneRepository.UpdateMilestonte(mile, majorId);
            var semester = await _semesterRepository.findActive();
            if(semester != null)
            {
                // xu li delivery
                var deli = await _deliverableRepository.GetByMileIdAndActiveSenmester(mile.Id);
                deli.Name = request.Name;
                deli.Description = request.Description;
                deli.Deadline = request.Deadline;
                // xu li cap nhat deli
                foreach (var item in deli.DeliveryItems)
                {
                    var x = mile.MilestoneItems.FirstOrDefault(x => x.Id == item.MilestoneItemId);
                    if (x != null)
                    {
                        item.Name = x.Name;
                        item.Description = x.Description;
                    }
                }
                // xu li xoa
                var deleteDelis = deli.DeliveryItems.Where(x => deleteItems.Any(y => y.Id == x.MilestoneItemId)).ToList();
                deleteDelis.ForEach(x => deli.DeliveryItems.Remove(x));
                // xu li them
                var addDelis = addItems.Select(x => new DeliveryItem
                {
                    Name = x.Name,
                    Description = x.Description,
                    DeliverableId = deli.Id,
                    MilestoneItemId = x.Id
                }).ToList();
                addDelis.ForEach(x => deli.DeliveryItems.Add(x));
                await _deliverableRepository.UpdateDeliverable(deli);
                Log log = new Log()
                {
                    Name = "Cập nhật milestone " + mile.Name,
                    EntityName = "Milestone",
                    EntityId = mile.Id,
                    Action = StringEnum.Update,
                    Description = mile.Description + mile.MilestoneItems.ToString(),
                    UserId = (int)user.Id,
                    CreateAt = DateTime.Now
                };
                Log deliLog = new Log
                {
                    Name = "Cập nhật Deliverable " + deli.Name,
                    EntityName = "Deliverable",
                    EntityId = deli.Id,
                    Action = StringEnum.Update,
                    Description = deli.Description + deli.DeliveryItems.ToString(),
                    UserId = (int)user.Id,
                    CreateAt = DateTime.Now
                };
                await _logService.AddRangeLogAsync(new List<Log>
            {
                log, deliLog
            });
            }
            var response = list.Adapt<List<MilestoneResponse>>();
            return ApiResponse<List<MilestoneResponse>>.Success(response);
        }
        public async Task<List<MilestonesDTO>> GetMilestonesByGroupIdAsync(int groupId)
        {
            var milestones = await _milestoneRepository.GetMilestonesByGroupIdAsync(groupId);

            return milestones.Select(m => new MilestonesDTO
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                Deadline = m.Deadline,
                CreateAt = m.CreateAt,
                CreateBy = m.CreateBy,
                MajorId = m.MajorId
            }).ToList();
        }
    }
}
