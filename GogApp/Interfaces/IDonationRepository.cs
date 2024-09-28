using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface IDonationRepository
{
    Task<IEnumerable<Donation>> GetDonationsByProjectId(int projectId);
    Task<bool> Add(Donation donation);
    Task<bool> SaveAsync();
}
