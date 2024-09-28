using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface IProjectVolunteerRepository
{
    Task<IEnumerable<ProjectVolunteer>> GetAllProjectVolunteersAsync(int projectId);
    Task<ProjectVolunteer?> GetVolunteerAsync(int projectId, string userId);
    Task<bool> AddVolunteerAsync(ProjectVolunteer projectVolunteer); // Updated
    Task<bool> DeleteVolunteerAsync(ProjectVolunteer projectVolunteer); // Updated
    Task<bool> SaveAsync(); // Updated
}
