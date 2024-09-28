using System;

namespace GogApp.Models;

public class ProjectVolunteer
{
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public string? AppUserId { get; set; }
    public AppUser? Volunteer { get; set; }

    public DateTime SignedUpAt { get; set; }
}
