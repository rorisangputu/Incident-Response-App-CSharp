using System;

namespace GogApp.ViewModels;

public class EditReportViewModel
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ProjectId { get; set; }
}
