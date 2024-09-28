using System;
using System.ComponentModel.DataAnnotations;
using GogApp.Models;

namespace GogApp.ViewModels;

public class AssignTaskVolunteerViewModel
{
    [Required]
    public int ProjectTaskId { get; set; }

    [Required]
    [Display(Name = "Select Volunteers")]
    public List<string>? SelectedVolunteerIds { get; set; } // List of selected volunteer IDs

    public IEnumerable<ProjectVolunteer>? AvailableVolunteers { get; set; } // List of volunteers to choose from
    public ProjectTask? ProjectTask { get; set; } // For displaying task details in the view

}
