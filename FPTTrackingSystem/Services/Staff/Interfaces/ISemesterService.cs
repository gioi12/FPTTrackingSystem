using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using DataTranferObjects.Staff.Semester;
using Entities.Models;
using FPTTrackingSystem.Wrappers;

namespace FPTTrackingSystem.Services.Staff.Interfaces
{
    public interface ISemesterService
    {
        Task<ApiResponse<SemesterActiveRes>> GetSemesterActiveAndMajors();
        Task<SemesterDTO> CreateSemesterAsync(SemesterCreateRequest request);
        Task<bool> IsOverlappingAsync(DateOnly start, DateOnly end);
        Task<List<SemesterDTO>> GetAllSemestersAsync();
        Task<SemesterDTO?> GetSemesterByIdAsync(int id);
        Task<SemesterDTO> UpdateSemesterAsync(int id, SemesterUpdateRequest request);
        Task<Semester?> GetSemesterByNow();
        Task<SemesterDeliveriesDTO?> GetMilestonesBySemester(int id);
        Task<SemesterDeliveriesDTO?> GetDeliveriesBySemester(int id);
        Task<Semester?> GetGroupsBySemester(int id);
        Task<ApiResponse<string>> AddVacationsAsync(List<SemesterVacationRequestDto> vacations);
        Task<ApiResponse<string>> UpdateSemesterVacationsAsync(int semesterId, List<SemesterUpdateVacationRequestDto> vacationDtos);
        Task<ApiResponse<List<SemesterVacationDto>>> GetBySemesterIdAsync(int semesterId);

        Task<ApiResponse<List<SemesterVacationDto>>> GetVacationsBySemesterAsync(int semesterId);
        Task<ApiResponse<SemesterDTO>> SyncSemesterByNameAsync(string semesterName);
    }
}
