﻿using AuthSystem.Areas.Identity.Data;
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
 //   public IEnumerable<object> Courses { get; internal set; }

public DbSet<AuthSystem.Models.Course> Course { get; set; } = default!;

public DbSet<AuthSystem.Models.UserCourse> UserCourse { get; set; } = default!;



public DbSet<AuthSystem.Models.Subject> Subject { get; set; } = default!;

public DbSet<AuthSystem.Models.Grade> Grade { get; set; } = default!;



}