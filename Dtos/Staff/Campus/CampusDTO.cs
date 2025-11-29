using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Staff.Campus
{
    public class CampusDTO
    {
    }

    public class SlotCampusDto
    {
        public int Id { get; set; }
        public string? NameSlot { get; set; }
        public string? StartAt { get; set; }
        public string? EndAt { get; set; }
    }

    public class CreateCampusDto
    {
        public string Name { get; set; } = null!;
    }

    public class UpdateCampusDto
    {
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }


    public class CampusDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<SlotCampusDto> Slots { get; set; } = new();
    }

    public class CampusAllDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
     /*   public List<SlotCampusDto> Slots { get; set; } = new();*/
    }

    public class UpdateSlotActiveRequest
    {
        public bool IsActive { get; set; }
    }


    public class SlotCreateDto
    {
        public string NameSlot { get; set; } = null!;
        public string StartAt { get; set; }
        public string EndAt { get; set; }
    }


}
