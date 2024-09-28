using System;
using GogApp.Models;

namespace GogApp.ViewModels;

public class UserDashboardViewModel
{
    public AppUser? User { get; set; }
    public ICollection<Project>? MyProjects { get; set; } // Projects the user manages
    public ICollection<Project>? VolunteeredProjects { get; set; } // Projects the user is volunteering for
    public ICollection<TaskVolunteer>? AssignedTasks { get; set; } // Tasks assigned to the user

}

