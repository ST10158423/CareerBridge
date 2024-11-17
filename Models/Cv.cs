using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class Cv
{
    public int Cvid { get; set; }

    public int? JobSeekerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? City { get; set; }

    public int? Experience { get; set; }

    public string? Certifications { get; set; }

    public string? EmploymentStatus { get; set; }

    public string? LevelOfEducation { get; set; }

    public string? AboutYou { get; set; }

    public string? Skills { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual JobSeeker? JobSeeker { get; set; }
}
