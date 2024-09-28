using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface IUserDashboardRepository
{
    Task<AppUser> GetUserWithProjectsAndTasksAsync(string userId); // Get user with their projects and tasks
    Task<AppUser> GetUserById(string id);
    Task<AppUser> GetUserByIdNoTracking(string id);
    bool Update(AppUser user);
    bool Save();
}
