using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobPortal1.Models;

public partial class JobSeeker
{
    public int JobSeekerId { get; set; }

    public int? UserId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required.")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string? PhoneNumber { get; set; }

    public bool? IsEmployed { get; set; }

    public bool? VisibilityConsent { get; set; }

    public virtual ICollection<Interest> Interests { get; set; } = new List<Interest>();

    public virtual ICollection<JobSeekerCertificate> JobSeekerCertificates { get; set; } = new List<JobSeekerCertificate>();

    public virtual ICollection<JobSeekerPersonalInfo> JobSeekerPersonalInfos { get; set; } = new List<JobSeekerPersonalInfo>();

    public virtual ICollection<JobSeekerSkill> JobSeekerSkills { get; set; } = new List<JobSeekerSkill>();

    public virtual ICollection<JobSeekerWorkExperience> JobSeekerWorkExperiences { get; set; } = new List<JobSeekerWorkExperience>();

    public virtual User? User { get; set; }
}
