using System;
using Microsoft.AspNetCore.Identity;

namespace GogApp.Models;

public class AppUser : IdentityUser
{
    public string? About { get; set; }
    // Projects the user manages (they are the creator/manager)
    public ICollection<Project>? MyProjects { get; set; }

    // Projects the user is volunteering for (not managing)
    public ICollection<ProjectVolunteer>? ProjectVolunteers { get; set; }

    // Tasks assigned to the user as a volunteer
    public ICollection<TaskVolunteer>? TaskVolunteers { get; set; }
}
