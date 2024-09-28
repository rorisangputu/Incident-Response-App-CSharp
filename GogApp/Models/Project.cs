using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GogApp.Models;

public class Project
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }

    // New properties added
    public string? Description { get; set; }
    public string? Details { get; set; }

    // Collection of reports related to the project
    public ICollection<Report>? Reports { get; set; }

    // Collection of donations related to the project
    public ICollection<Donation>? Donations { get; set; }

    [ForeignKey("AppUser")]
    public string ManagerId { get; set; }
    public AppUser? Manager { get; set; }

    public ICollection<ProjectVolunteer>? ProjectVolunteers { get; set; }
    public ICollection<ProjectTask>? ProjectTasks { get; set; }
}
