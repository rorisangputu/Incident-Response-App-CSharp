using System;
using GogApp.Models;

namespace GogApp.ViewModels;

public class TaskDetailViewModel
{
    public ProjectTask? Task { get; set; }
    public IEnumerable<TaskVolunteer>? AssignedVolunteers { get; set; }
    public DateTime? AssignedAt => Task?.AssignedAt;
}
