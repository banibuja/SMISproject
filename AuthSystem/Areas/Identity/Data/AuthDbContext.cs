using AuthSystem.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AuthSystem.Models;

namespace AuthSystem.Data;

public class AuthDbContext : IdentityDbContext<ApplicationUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    /*  protected override void OnModelCreating(ModelBuilder builder)
      {
          base.OnModelCreating(builder);

          // If you want to avoid cascading delete
          builder.Entity<SemesterRegistration>()
              .HasOne(sr => sr.User)
              .WithMany()
              .HasForeignKey(sr => sr.UserId)
              .OnDelete(DeleteBehavior.Restrict);  // or use DeleteBehavior.SetNull if needed
      } */


    public DbSet<AuthSystem.Models.Department> Department { get; set; } = default!;


    public DbSet<AuthSystem.Models.UserSubject> UserSubject { get; set; } = default!;



    public DbSet<AuthSystem.Models.Subject> Subject { get; set; } = default!;

    public DbSet<AuthSystem.Models.Grade> Grade { get; set; } = default!;

    public DbSet<ExamPeriod> ExamPeriods { get; set; }

    public DbSet<AuthSystem.Models.ExamSubmission> ExamSubmission { get; set; } = default!;

    public DbSet<AuthSystem.Models.Location> Location { get; set; } = default!;

    public DbSet<AuthSystem.Models.Semester> Semester { get; set; } = default!;

    public DbSet<AuthSystem.Models.Schedule> Schedule { get; set; } = default!;

    public DbSet<AuthSystem.Models.StudentSemester> StudentSemester { get; set; } = default!;

    public DbSet<Log> Logs { get; set; }  // Shtoni këtë linjë

    public DbSet<Exam> Exam { get; set; }



}