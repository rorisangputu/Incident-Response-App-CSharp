using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace GogApp.Models;

public class Donation
{
    public int Id { get; set; }
    public string? Item { get; set; }
    public int Quantity { get; set; }
    public DateTime DonatedAt { get; set; }

    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }

    [ForeignKey("AppUser")]
    public string? AppUserId { get; set; }
    public AppUser? User { get; set; }
}
