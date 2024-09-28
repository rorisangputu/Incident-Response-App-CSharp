using System;
using System.ComponentModel.DataAnnotations;

namespace GogApp.ViewModels;

public class CreateProjectTaskViewModel
{
    public int ProjectId { get; set; } // This will be used to associate the task with a specific project

    [Required(ErrorMessage = "Task title is required.")]
    [StringLength(100, ErrorMessage = "Task title cannot be longer than 100 characters.")]
    public string? Title { get; set; }

    [Display(Name = "Assigned At")]
    [DataType(DataType.Date)]
    public DateTime? AssignedAt { get; set; }

    [Display(Name = "Completed At")]
    [DataType(DataType.Date)]
    public DateTime? CompletedAt { get; set; }

}
