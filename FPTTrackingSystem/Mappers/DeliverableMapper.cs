using DataTranferObjects.Enum;
using DataTranferObjects.Staff.Request;
using DataTranferObjects.Staff.Response;
using Entities.Models;
using Mapster;

namespace FPTTrackingSystem.Mappers
{
    public class DeliverableMapper
    {
        public static void ToDeliverableResponse()
        {
            TypeAdapterConfig<DeliveryItem, DeliverableItemRes>.NewConfig()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.Name, src => src.Name)
            .Map(dest => dest.Description, src => src.Description);

            TypeAdapterConfig<Deliverable, DeliverableRes>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Description, src => src.Description)
                .Map(dest => dest.Deadline, src => src.Deadline)
                .Map(dest => dest.DeliveryItems, src => src.DeliveryItems.Adapt<List<DeliverableItemRes>>());

            TypeAdapterConfig<Deliverable, GroupDeliverableRes>.NewConfig()
               .Map(dest => dest.Id, src => src.Id)
               .Map(dest => dest.Name, src => src.Name)
               .Map(dest => dest.Description, src => src.Description)
               .Map(dest => dest.Deadline, src => src.Deadline)
               .Map(dest => dest.Status, src => src.DeliverableGroups.FirstOrDefault() != null ? src.DeliverableGroups.FirstOrDefault().Status : ProgressEnum.Unsubmitted)
               .Map(dest => dest.DeliveryItems, src => src.DeliveryItems.Adapt<List<DeliverableItemRes>>());

            TypeAdapterConfig<Deliverable, DeliverableDetailRes>.NewConfig()
               .Map(dest => dest.Id, src => src.Id)
               .Map(dest => dest.Name, src => src.Name)
               .Map(dest => dest.Description, src => src.Description)
               .Map(dest => dest.Deadline, src => src.Deadline)
               .Map(dest => dest.Status, src => src.DeliverableGroups.FirstOrDefault() != null ? src.DeliverableGroups.FirstOrDefault().Status : ProgressEnum.Unsubmitted)
               .Map(dest => dest.DeliveryItems, src => src.DeliveryItems.Adapt<List<DeliverableItemRes>>());
        }
    }
}
