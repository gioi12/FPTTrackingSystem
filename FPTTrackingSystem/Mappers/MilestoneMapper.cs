using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using Mapster;

namespace FPTTrackingSystem.Mappers
{
    public class MilestoneMapper
    {
        public static void ToMilestoneResponse()
        {
            TypeAdapterConfig<MilestoneItem, MilestoneItemRequest>.NewConfig()
            .Map(dest => dest.id, src => src.Id)
            .Map(dest => dest.name, src => src.Name)
            .Map(dest => dest.description, src => src.Description);

            TypeAdapterConfig<Milestone, MilestoneResponse>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Deadline, src => src.Deadline)
                .Map(dest => dest.CreateAt, src => src.CreateAt)
                .Map(dest => dest.UserCreatedName, src => src.CreateByNavigation != null ? src.CreateByNavigation.Fullname : "")
                .Map(dest => dest.MajorName, src => src.Major.Name)
                .Map(dest => dest.Items, src => src.MilestoneItems.Adapt<List<MilestoneItemRequest>>());
        }
    }
}
