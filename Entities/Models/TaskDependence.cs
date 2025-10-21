using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class TaskDependence
{
    public int TaskId { get; set; }

    public int TaskReferenceId { get; set; }
}
