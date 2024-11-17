using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class Certificate
{
    public int CertificateId { get; set; }

    public int? Cvid { get; set; }

    public string? CertificateName { get; set; }

    public string? FilePath { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Cv? Cv { get; set; }
}
