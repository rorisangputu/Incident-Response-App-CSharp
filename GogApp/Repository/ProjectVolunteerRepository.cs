using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class ProjectVolunteerRepository : IProjectVolunteerRepository
{
    private readonly ApplicationDbContext context;

    public ProjectVolunteerRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

    public async Task<bool> AddVolunteerAsync(ProjectVolunteer projectVolunteer)
    {
        context.Add(projectVolunteer);
        return await SaveAsync();
    }

    public async Task<bool> DeleteVolunteerAsync(ProjectVolunteer projectVolunteer)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ProjectVolunteer>> GetAllProjectVolunteersAsync(int projectId)
    {
        return await context.ProjectVolunteers
            .Where(pv => pv.Id == projectId) // Filter by ProjectId
            .Include(pv => pv.Volunteer) // Include related AppUser (volunteer)
            .Include(pv => pv.Project) // Include related Project
            .ToListAsync();
    }


    public async Task<ProjectVolunteer?> GetVolunteerAsync(int projectId, string userId)
    {
        return await context.ProjectVolunteers
           .FirstOrDefaultAsync(v => v.Id == projectId && v.Volunteer.Id == userId);
    }

    public async Task<bool> SaveAsync()
    {
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }
}
