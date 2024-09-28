using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class UserDashboardRepository : IUserDashboardRepository
{
    private readonly ApplicationDbContext context;

    public UserDashboardRepository(ApplicationDbContext dBcontext)
    {
        context = dBcontext;
    }

    public async Task<AppUser> GetUserById(string id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<AppUser> GetUserByIdNoTracking(string id)
    {
        return await context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
    }

    public async Task<AppUser> GetUserWithProjectsAndTasksAsync(string userId)
    {
        return await context.Users
            .Include(u => u.MyProjects)
            .Include(u => u.ProjectVolunteers)
                .ThenInclude(pv => pv.Project) // Include the Project related to volunteering
            .Include(u => u.TaskVolunteers)
                .ThenInclude(tv => tv.ProjectTask) // Include tasks assigned to the user
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public bool Update(AppUser user)
    {
        context.Users.Update(user);
        return Save();
    }

    public bool Save()
    {
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }
}
