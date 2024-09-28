using System;

namespace GogApp.Models;

public class TaskVolunteer
{
    public int ProjectTaskId { get; set; }
    public ProjectTask? ProjectTask { get; set; }

    // Use string for AppUserId to match the IdentityUser primary key
    public string? AppUserId { get; set; }
    public AppUser? Volunteer { get; set; }
}
