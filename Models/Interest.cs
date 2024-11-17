using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class Interest
{
    public int InterestId { get; set; }

    public int? EmployerId { get; set; }

    public int? JobSeekerId { get; set; }

    public DateTime? DateExpressed { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Employer? Employer { get; set; }

    public virtual JobSeeker? JobSeeker { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
