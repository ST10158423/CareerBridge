using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int? InterestId { get; set; }

    public decimal PaymentAmount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public virtual Interest? Interest { get; set; }
}
