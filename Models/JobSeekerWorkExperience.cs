using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobPortal1.Models;

public partial class JobSeekerWorkExperience
{
    public int ExpId { get; set; }

    public int? JobSeekerId { get; set; }

    public string? CompanyName { get; set; }

    public string? CompanyAddress { get; set; }

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits.")]
    public string? CompanyPhoneNumber { get; set; }

    public virtual JobSeeker? JobSeeker { get; set; }
}
