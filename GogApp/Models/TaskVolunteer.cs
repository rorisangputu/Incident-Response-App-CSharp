using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GogApp.Models;

public class TaskVolunteer
{
    [Key]
    public int TaskVolunteerId { get; set; }

    [ForeignKey("ProjectTask")]
    public int ProjectTaskId { get; set; }
    public ProjectTask? ProjectTask { get; set; }


    // Use string for AppUserId to match the IdentityUser primary key
    [ForeignKey("AppUser")]
    public string? AppUserId { get; set; }
    public AppUser? Volunteer { get; set; }
}
