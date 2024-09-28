using System;
using System.ComponentModel.DataAnnotations;

namespace GogApp.ViewModels;

public class CreateProjectViewModel
{
    [Required]
    [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
    public string? Title { get; set; }
    [Required]
    [StringLength(100, ErrorMessage = "The title cannot exceed 100 characters.")]
    public string? Description { get; set; }

    public string? Details { get; set; }

    [Required]
    public string? ManagerId { get; set; }
}
