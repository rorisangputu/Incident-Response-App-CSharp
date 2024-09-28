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
        base.OnModelCreating(modelBuilder);  // Call this to ensure Identity tables are created

        // Define relationships, keys, or table mappings if needed
        modelBuilder.Entity<Project>().ToTable("Projects");
        modelBuilder.Entity<ProjectTask>().ToTable("Tasks");
        modelBuilder.Entity<ProjectVolunteer>().ToTable("ProjectVolunteers");
        modelBuilder.Entity<TaskVolunteer>().ToTable("TaskVolunteers");
        modelBuilder.Entity<Report>().ToTable("Reports");
        modelBuilder.Entity<Donation>().ToTable("Donations");

        // Add any specific configurations or constraints
    }

}
