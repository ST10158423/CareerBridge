using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class JobSeekerSkill
{
    public int SkillId { get; set; }

    public int? JobSeekerId { get; set; }

    public string? SkillName { get; set; }

    public string SkillLevel { get; set; } = null!;

    public virtual JobSeeker? JobSeeker { get; set; }
}
