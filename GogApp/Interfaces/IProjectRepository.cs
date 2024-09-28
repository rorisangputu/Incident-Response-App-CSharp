using System;
using GogApp.Models;

namespace GogApp.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAll();
    Task<Project> GetByIdAsync(int id);
    Task<Project> GetByIdAsyncAsNoTracking(int id);
    bool Add(Project project);
    bool Update(Project project);
    bool Delete(Project project);
    bool Save();
}
