using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? UserId { get; set; }

    public int? JobSeekerId { get; set; }

    public int? EmployerId { get; set; }

    public string NotificationType { get; set; } = null!;

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public bool? IsActionRequired { get; set; }

    public int? ProcessedByAdminId { get; set; }

    public virtual User? ProcessedByAdmin { get; set; }
}
