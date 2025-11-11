using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTranferObjects.Enum
{
    public enum RoleEnum
    {
        Student = 1,
        Supervior = 2,
        SuperviorHead = 3,
        Staff = 4,
        StudentLead = 6,
        Admin = 5,
    }

    public enum StatusTask
    {
        ToDo = 1,
        InProgress = 2,
        Done = 3
    }

    public enum TaskPriority
    {
        Normal = 1,
        Medium = 2,
        High = 3
    }
}
