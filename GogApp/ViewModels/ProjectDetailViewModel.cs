using System;
using GogApp.Models;

namespace GogApp.ViewModels;

public class ProjectDetailViewModel
{
    // Project details
    public Project? Project { get; set; }

    // List of volunteers for the project
    public IEnumerable<ProjectVolunteer>? Volunteers { get; set; }
}
