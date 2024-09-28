using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GogApp.Models;

public class Report
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }

    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
}
