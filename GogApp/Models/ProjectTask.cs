using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GogApp.Models;

public class ProjectTask
{
    public int Id { get; set; }
    public string? Title { get; set; }

    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    public ICollection<TaskVolunteer>? TaskVolunteers { get; set; }

    public DateTime? AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
