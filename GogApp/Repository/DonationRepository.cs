using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class DonationRepository : IDonationRepository
{
    private readonly ApplicationDbContext context;

    public DonationRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }
    public async Task<bool> Add(Donation donation)
    {
        context.Add(donation);
        return await SaveAsync();
    }

    public async Task<IEnumerable<Donation>> GetDonationsByProjectId(int projectId)
    {
        return await context.Donations
            .Where(d => d.ProjectId == projectId)
            .ToListAsync();
    }

    public async Task<bool> SaveAsync()
    {
        var saved = await context.SaveChangesAsync();
        return saved > 0 ? true : false;
    }
}
