using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface ITaskVolunteerRepository
{
    Task<IEnumerable<TaskVolunteer>> GetTaskVolunteersByTaskIdAsync(int taskId);
    Task<bool> AddTaskVolunteerAsync(TaskVolunteer taskVolunteer); // Updated
    Task<bool> DeleteVolunteerAsync(TaskVolunteer taskVolunteer); // Updated
    Task<bool> SaveAsync(); // Updated
}
