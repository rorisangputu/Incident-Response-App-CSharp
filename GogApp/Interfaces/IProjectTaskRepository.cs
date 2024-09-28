using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface IProjectTaskRepository
{
    Task<IEnumerable<ProjectTask>> GetAllTasksAsync();
    Task<ProjectTask> GetTaskByIdAsync(int Id);
    Task<ProjectTask> GetTaskByIdAsyncNoTracking(int Id);
    bool Add(ProjectTask task);
    bool Update(ProjectTask task);
    bool Delete(ProjectTask task);
    bool Save();
    Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(int projectId);
}
