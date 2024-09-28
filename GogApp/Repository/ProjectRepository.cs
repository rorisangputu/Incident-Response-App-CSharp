using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext context;
    public ProjectRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }

    public bool Add(Project project)
    {
        context.Add(project);
        return Save();
    }

    public async Task<IEnumerable<Project>> GetAll()
    {
        return await context.Projects.ToListAsync();
    }

    public async Task<Project> GetByIdAsync(int id)
    {
        return await context.Projects
                    .Include(y => y.ProjectTasks)
                    .ThenInclude(t => t.TaskVolunteers) // Include task volunteers
                    .Include(m => m.Manager)
                    .Include(r => r.Reports)
                    .Include(d => d.Donations)
                    .Include(v => v.ProjectVolunteers) // Include project volunteers
                    .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<Project> GetByIdAsyncAsNoTracking(int id)
    {
        return await context.Projects.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id);
    }

    public bool Save()
    {
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }

    public bool Update(Project project)
    {
        context.Update(project);
        return Save();
    }

    public bool Delete(Project project)
    {
        // Manually remove the project volunteers from the database before deleting the project
        if (project.ProjectVolunteers != null)
        {
            context.ProjectVolunteers.RemoveRange(project.ProjectVolunteers);
        }

        // Manually remove task volunteers for all project tasks
        foreach (var task in project.ProjectTasks)
        {
            if (task.TaskVolunteers != null)
            {
                context.TaskVolunteers.RemoveRange(task.TaskVolunteers);
            }
        }

        // Finally, remove the project itself
        context.Projects.Remove(project);
        return Save();
    }
}
