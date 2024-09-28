using System;

namespace GogApp.ViewModels;

public class SignUpProjectVolunteerViewModel
{
    public int ProjectId { get; set; }  // The ID of the project
    public string? ProjectTitle { get; set; }  // The title of the project

    public string? VolunteerId { get; set; }  // The ID of the volunteer (AppUser)
    public string? VolunteerName { get; set; }  // The name or username of the volunteer

    public DateTime SignedUpAt { get; set; }  // When the volunteer signed up for the project

}
