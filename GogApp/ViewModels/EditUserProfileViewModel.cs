using System;
using System.ComponentModel.DataAnnotations;

namespace GogApp.ViewModels;

public class EditUserProfileViewModel
{
    [Required]
    [Display(Name = "Username")]
    public string? UserName { get; set; }

    public string? About { get; set; }


}
