using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GogApp.Models;

public class ProjectVolunteer
{
    [Key]
    public int Id { get; set; }
    public Project? Project { get; set; }
    public AppUser? Volunteer { get; set; }

    public DateTime SignedUpAt { get; set; }
}
