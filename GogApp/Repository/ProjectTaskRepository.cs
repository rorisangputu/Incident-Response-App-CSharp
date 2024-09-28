using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class ProjectTaskRepository : IProjectTaskRepository
{
    private readonly ApplicationDbContext context;

    public ProjectTaskRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

    public bool Add(ProjectTask task)
    {
        context.Add(task);
        return Save();
    }

    public bool Update(ProjectTask task)
    {
        context.Update(task);
        return Save();
    }

    public bool Delete(ProjectTask task)
    {
        context.Remove(task);
        return Save();
    }

    public bool Save()
    {
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public async Task<IEnumerable<ProjectTask>> GetAllTasksAsync()
    {
        return await context.Tasks.Include(t => t.Project).ToListAsync();
    }

    public async Task<ProjectTask> GetTaskByIdAsync(int Id)
    {
        return await context.Tasks.Include(t => t.Project).FirstOrDefaultAsync(t => t.Id == Id);
    }

    public async Task<ProjectTask> GetTaskByIdAsyncNoTracking(int Id)
    {
        return await context.Tasks.Include(t => t.Project).AsNoTracking().FirstOrDefaultAsync(t => t.Id == Id);
    }

    public async Task<IEnumerable<ProjectTask>> GetTasksByProjectIdAsync(int projectId)
    {
        return await context.Tasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();
    }
}
