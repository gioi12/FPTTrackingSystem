using Azure;
using DataTranferObjects.Group;
using Microsoft.EntityFrameworkCore;
using Repositories.GroupRepository;

namespace FPTTrackingSystem.Services.Group
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;

        public GroupService(IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        public async Task<PagedResponse<GroupDto>> GetGroupsAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _groupRepository.GetGroupsQuery();
            var total = await _groupRepository.CountAsync(query);

            var groups = await query
                .OrderBy(g => g.Id) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                 .Select(g => new GroupDto
                 {
                     Id = g.Id.ToString(),
                     CourseCode = g.Name,
                     Term = g.Semester != null ? g.Semester.Name : "",
                     Major = g.Major != null ? g.Major.Name : "",
                     StudentCount = g.GroupUsers.Count(gu => gu.User.Account.RoleId == 1),
                     Supervisor = g.GroupUsers
                        .Where(gu => gu.User.Account.RoleId == 2 || gu.User.Account.RoleId == 3)
                        .Select(gu => gu.User.Fullname)
                        .ToList(),
                     // SubmittedDocs = g.Milestones.Any(m => m.Attachments.Any()) 
                     SubmittedDocs = false
                 })
                  .ToListAsync();

            return new PagedResponse<GroupDto>
            {
                Status = 200,
                Message = "Lấy thành công",
                Data = new PagedData<GroupDto>
                {
                    Items = groups,
                    Total = total
                }
            };
        }

        public async Task<ApiResponse<GroupDetailDto>> GetGroupByIdAsync(int id)
        {
            var group = await _groupRepository.GetByIdAsync(id);
            if (group == null)
            {
                return new ApiResponse<GroupDetailDto>
                {
                    Status = 404,
                    Message = "Không tìm thấy nhóm",
                    Data = null
                };
            }
             
            var dto = new GroupDetailDto
            {
                Id = group.Id.ToString(),
                ProjectName = group.Name,
                Supervisors = group.GroupUsers
                    .Where(gu => gu.User.Account.RoleId == 2 || gu.User.Account.RoleId == 3)
                    .Select(gu => gu.User.Fullname)
                    .ToList(),
                Status = group.Status.Name, 
                Risk = "Low",           
                Students = group.GroupUsers
                    .Where(gu => gu.User.Account.RoleId == 1)
                    .Select(gu => new StudentDto
                    {
                        Id = gu.User.RollNumber,
                        Name = gu.User.Fullname,
                        Role = gu.User.Account.Role.Name
                    }).ToList(),
                ActivityLog = null,
            };

            return new ApiResponse<GroupDetailDto>
            {
                Status = 200,
                Message = "Lấy thành công",
                Data = dto
            };
        }

        public async Task<ApiResponse<List<DashBoardGroupDto>>> GetMajorGroupTotalsAsync()
        {
            var data = await _groupRepository.GetMajorGroupTotalsAsync();
            return new ApiResponse<List<DashBoardGroupDto>>(200, "Lấy thành công", data);
        }


    }

}
