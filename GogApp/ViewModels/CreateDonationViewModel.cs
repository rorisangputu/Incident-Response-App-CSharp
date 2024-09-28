using System;

namespace GogApp.ViewModels;

public class CreateDonationViewModel
{
    public string Item { get; set; }
    public int Quantity { get; set; }
    public DateTime DonatedAt { get; set; }


    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }

    public string? AppUserId { get; set; }
}