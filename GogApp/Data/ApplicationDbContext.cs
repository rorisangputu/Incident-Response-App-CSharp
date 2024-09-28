using System;
using GogApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Data;

public class ApplicationDbContext : IdentityDbContext<AppUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<ProjectVolunteer> ProjectVolunteers { get; set; }
    public DbSet<TaskVolunteer> TaskVolunteers { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<Donation> Donations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure many-to-many relationships for Project Volunteers
        modelBuilder.Entity<ProjectVolunteer>()
            .HasKey(pv => new { pv.ProjectId, pv.AppUserId });

        modelBuilder.Entity<ProjectVolunteer>()
            .HasOne(pv => pv.Project)
            .WithMany(p => p.ProjectVolunteers)
            .HasForeignKey(pv => pv.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete ProjectVolunteers when a Project is deleted

        modelBuilder.Entity<ProjectVolunteer>()
            .HasOne(pv => pv.Volunteer)
            .WithMany(v => v.ProjectVolunteers)
            .HasForeignKey(pv => pv.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when a Volunteer is deleted

        // Configure many-to-many relationships for Task Volunteers
        modelBuilder.Entity<TaskVolunteer>()
            .HasKey(tv => new { tv.ProjectTaskId, tv.AppUserId });

        modelBuilder.Entity<TaskVolunteer>()
            .HasOne(tv => tv.ProjectTask)
            .WithMany(t => t.TaskVolunteers)
            .HasForeignKey(tv => tv.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete TaskVolunteers when a ProjectTask is deleted

        modelBuilder.Entity<TaskVolunteer>()
            .HasOne(tv => tv.Volunteer)
            .WithMany(v => v.TaskVolunteers)
            .HasForeignKey(tv => tv.AppUserId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete when a Volunteer is deleted

        // Configure cascade delete for ProjectTasks when a Project is deleted
        modelBuilder.Entity<Project>()
            .HasMany(p => p.ProjectTasks)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete ProjectTasks when a Project is deleted

        // Ensure TaskVolunteers are also deleted when a ProjectTask is deleted
        modelBuilder.Entity<ProjectTask>()
            .HasMany(t => t.TaskVolunteers)
            .WithOne(tv => tv.ProjectTask)
            .HasForeignKey(tv => tv.ProjectTaskId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete TaskVolunteers when a ProjectTask is deleted

        // Configure cascade delete for Reports when a Project is deleted
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Reports)
            .WithOne(r => r.Project)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete Reports when a Project is deleted

        // Configure cascade delete for Donations when a Project is deleted
        modelBuilder.Entity<Project>()
            .HasMany(p => p.Donations)
            .WithOne(d => d.Project)
            .HasForeignKey(d => d.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);  // Cascade delete Donations when a Project is deleted

        base.OnModelCreating(modelBuilder);
    }
}
