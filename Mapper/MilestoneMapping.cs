using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mappers
{
    internal class MilestoneMapping
    {
        public static void ToMilestoneResponse()
        {
            TypeAdapterConfig<Milestone, MilestoneResponse>.NewConfig()
                .Map(dest => dest.CreateAt, src => DateTime.Now)
                .Map(dest => dest.UserCreatedName, src => src.CreateByNavigation != null ? src.CreateByNavigation.Fullname : "")
                .Map(dest => dest.MajorName, src => src.Major.Name)
                .Map(dest => dest.SemesterName, src => src.Semester.Name);
        }
    }
}
