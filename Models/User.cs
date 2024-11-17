using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JobPortal1.Models
{
    public partial class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required.")]
        public string PasswordHash { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "User type is required.")]
        public string UserType { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateTime? DateCreated { get; set; }

        public DateTime? LastLogin { get; set; }

        public virtual ICollection<Employer> Employers { get; set; } = new List<Employer>();

        public virtual ICollection<JobSeeker> JobSeekers { get; set; } = new List<JobSeeker>();

        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
