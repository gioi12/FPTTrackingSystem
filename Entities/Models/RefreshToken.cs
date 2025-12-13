using System;
using System.Collections.Generic;

namespace Entities.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public string? Token { get; set; }

    public string? Device { get; set; }

    public string? IpAddress { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? ExpireAt { get; set; }

    public bool? IsRevoked { get; set; }

    public virtual User? User { get; set; }
}
