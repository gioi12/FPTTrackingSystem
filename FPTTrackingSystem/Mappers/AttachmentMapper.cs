using DataTranferObjects.Common.Response;
using Entities.Models;
using Mapster;

namespace FPTTrackingSystem.Mappers
{
    public class AttachmentMapper
    {
        public static void ToAttachmentResponse()
        {
            TypeAdapterConfig<Attachment, AttachmentRes>.NewConfig()
              .Map(dest => dest.Id, src => src.Id)
              .Map(dest => dest.FileName, src => src.FileName)
              .Map(dest => dest.Path, src => src.FilePath)
              .Map(dest => dest.UserName, src => src.User.Fullname)
              .Map(dest => dest.CreateAt, src => src.CreateAt);
        }
    }
}
