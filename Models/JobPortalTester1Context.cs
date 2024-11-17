using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace JobPortal1.Models;

public partial class JobPortalTester1Context : DbContext
{
    public JobPortalTester1Context()
    {
    }

    public JobPortalTester1Context(DbContextOptions<JobPortalTester1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Employer> Employers { get; set; }

    public virtual DbSet<Interest> Interests { get; set; }

    public virtual DbSet<JobSeeker> JobSeekers { get; set; }

    public virtual DbSet<JobSeekerCertificate> JobSeekerCertificates { get; set; }

    public virtual DbSet<JobSeekerPersonalInfo> JobSeekerPersonalInfos { get; set; }

    public virtual DbSet<JobSeekerSkill> JobSeekerSkills { get; set; }

    public virtual DbSet<JobSeekerWorkExperience> JobSeekerWorkExperiences { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=labVMH8OX\\SQLEXPRESS;Initial Catalog=JobPortalTester1;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employer>(entity =>
        {
            entity.HasKey(e => e.EmployerId).HasName("PK__Employer__CA445241BC48C819");

            entity.Property(e => e.EmployerId).HasColumnName("EmployerID");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactPersonFirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.ContactPersonLastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.EmployerType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Company");
            entity.Property(e => e.Industry)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Suburb)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Employers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Employers__UserI__4B422AD5");
        });

        modelBuilder.Entity<Interest>(entity =>
        {
            entity.HasKey(e => e.InterestId).HasName("PK__Interest__20832C67CE479BB8");

            entity.Property(e => e.DateExpressed)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.Employer).WithMany(p => p.Interests)
                .HasForeignKey(d => d.EmployerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Interests_Employer");

            entity.HasOne(d => d.JobSeeker).WithMany(p => p.Interests)
                .HasForeignKey(d => d.JobSeekerId)
                .HasConstraintName("FK_Interests_JobSeeker");
        });

        modelBuilder.Entity<JobSeeker>(entity =>
        {
            entity.HasKey(e => e.JobSeekerId).HasName("PK__JobSeeke__89113A8C5DFF77D5");

            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsEmployed).HasDefaultValue(false);
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.VisibilityConsent).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.JobSeekers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__JobSeeker__UserI__373B3228");
        });

        modelBuilder.Entity<JobSeekerCertificate>(entity =>
        {
            entity.HasKey(e => e.SupDocumentsId).HasName("PK__JobSeeke__8D99C21DB3D87793");

            entity.Property(e => e.SupDocumentsId).HasColumnName("SupDocumentsID");
            entity.Property(e => e.DocName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.DocUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("DocURL");
            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.JobSeeker).WithMany(p => p.JobSeekerCertificates)
                .HasForeignKey(d => d.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__JobSeeker__JobSe__43A1090D");
        });

        modelBuilder.Entity<JobSeekerPersonalInfo>(entity =>
        {
            entity.HasKey(e => e.PersonalInfoId).HasName("PK__JobSeeke__EA7BF0C4CEF0C71E");

            entity.ToTable("JobSeekerPersonalInfo");

            entity.Property(e => e.PersonalInfoId).HasColumnName("PersonalInfoID");
            entity.Property(e => e.AboutYou).HasColumnType("text");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");
            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.LastUpdated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LevelOfEducation)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Province)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Suburb)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.JobSeeker).WithMany(p => p.JobSeekerPersonalInfos)
                .HasForeignKey(d => d.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__JobSeeker__JobSe__3BFFE745");
        });

        modelBuilder.Entity<JobSeekerSkill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__JobSeeke__DFA091E7145D8A58");

            entity.Property(e => e.SkillId).HasColumnName("SkillID");
            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");
            entity.Property(e => e.SkillLevel)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.SkillName)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.JobSeeker).WithMany(p => p.JobSeekerSkills)
                .HasForeignKey(d => d.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__JobSeeker__JobSe__3FD07829");
        });

        modelBuilder.Entity<JobSeekerWorkExperience>(entity =>
        {
            entity.HasKey(e => e.ExpId).HasName("PK__JobSeeke__45B117C752D7592F");

            entity.ToTable("JobSeekerWorkExperience");

            entity.Property(e => e.ExpId).HasColumnName("ExpID");
            entity.Property(e => e.CompanyAddress)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyName)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.CompanyPhoneNumber)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");

            entity.HasOne(d => d.JobSeeker).WithMany(p => p.JobSeekerWorkExperiences)
                .HasForeignKey(d => d.JobSeekerId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__JobSeeker__JobSe__467D75B8");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32EEE58072");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmployerId).HasColumnName("EmployerID");
            entity.Property(e => e.IsActionRequired).HasDefaultValue(true);
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.JobSeekerId).HasColumnName("JobSeekerID");
            entity.Property(e => e.Message).HasColumnType("text");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ProcessedByAdminId).HasColumnName("ProcessedByAdminID");
            entity.Property(e => e.ReadAt).HasColumnType("datetime");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.ProcessedByAdmin).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.ProcessedByAdminId)
                .HasConstraintName("FK__Notificat__Proce__5D60DB10");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__9B556A58F2C3AABA");

            entity.Property(e => e.PaymentId).HasColumnName("PaymentID");
            entity.Property(e => e.InterestId).HasColumnName("InterestID");
            entity.Property(e => e.PaymentAmount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Success");

            entity.HasOne(d => d.Interest).WithMany(p => p.Payments)
                .HasForeignKey(d => d.InterestId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Payments__Intere__56B3DD81");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACAF258E9A");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E49E3C7F5A").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534C6FE4D21").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.LastLogin).HasColumnType("datetime");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.UserType)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Username)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
