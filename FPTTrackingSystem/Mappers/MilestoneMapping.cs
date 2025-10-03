using DataTranferObjects.Staff.Response;
using Entities.Models;
using Mapster;

namespace FPTTrackingSystem.Mappers
{
    public class MilestoneMapping
    {
        public static void ToMilestoneResponse()
        {
            TypeAdapterConfig<Milestone, MilestoneResponse>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Deadline, src => src.Deadline)
                .Map(dest => dest.CreateAt, src => DateTime.Now)
                .Map(dest => dest.StartAt, src => src.StartAt)
                .Map(dest => dest.EndAt, src => src.EndAt)
                .Map(dest => dest.UserCreatedName, src => src.CreateByNavigation != null ? src.CreateByNavigation.Fullname : "")
                .Map(dest => dest.MajorName, src => src.Major.Name)
                .Map(dest => dest.SemesterName, src => src.Semester.Name);
        }
      
    }
}
