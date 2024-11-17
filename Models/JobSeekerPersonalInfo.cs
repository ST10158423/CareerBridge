using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobPortal1.Models;

public partial class JobSeekerPersonalInfo
{
    public int PersonalInfoId { get; set; }

    public int? JobSeekerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "City is required.")]
    public string? City { get; set; }

    [Required(ErrorMessage = "Province is required.")]
    public string? Province { get; set; }

    [Required(ErrorMessage = "Suburb is required.")]
    public string? Suburb { get; set; }

    public string LevelOfEducation { get; set; } = null!;

    public string? AboutYou { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual JobSeeker? JobSeeker { get; set; }
}
