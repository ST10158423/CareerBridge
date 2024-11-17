using System;
using System.Collections.Generic;

namespace JobPortal1.Models;

public partial class JobSeekerCertificate
{
    public int SupDocumentsId { get; set; }

    public int? JobSeekerId { get; set; }

    public string? DocName { get; set; }

    public string? DocUrl { get; set; }

    public DateTime? UploadDate { get; set; }

    public virtual JobSeeker? JobSeeker { get; set; }
}
