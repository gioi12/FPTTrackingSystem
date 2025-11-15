using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class MailSetting
{
    public int Id { get; set; }

    public string? Mail { get; set; }

    public string? DisplayName { get; set; }

    public string? Password { get; set; }

    public string? Host { get; set; }

    public int? Port { get; set; }
}
