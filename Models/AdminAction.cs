using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class AdminAction
{
    public int ActionId { get; set; }

    public int? AdminId { get; set; }

    public int? TargetUserId { get; set; }

    public string? ActionType { get; set; }

    public string? ActionDetails { get; set; }

    public DateTime? ActionDate { get; set; }

    public virtual User? Admin { get; set; }

    public virtual User? TargetUser { get; set; }
}
