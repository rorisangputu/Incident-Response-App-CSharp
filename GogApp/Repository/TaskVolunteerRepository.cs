using System;
using GogApp.Data;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;

namespace GogApp.Repository;

public class TaskVolunteerRepository : ITaskVolunteerRepository
{
    private readonly ApplicationDbContext context;

    public TaskVolunteerRepository(ApplicationDbContext dbContext)
    {
        context = dbContext;
    }
    public async Task<bool> AddTaskVolunteerAsync(TaskVolunteer taskVolunteer)
    {
        context.Add(taskVolunteer);
        return await SaveAsync();
    }

    public Task<bool> DeleteVolunteerAsync(TaskVolunteer taskVolunteer)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<TaskVolunteer>> GetTaskVolunteersByTaskIdAsync(int taskId)
    {
        return await context.TaskVolunteers.Where(tv => tv.ProjectTaskId == taskId)
        .Include(tv => tv.Volunteer)
        .Include(tv => tv.ProjectTask)
        .ToListAsync();
    }

    public async Task<bool> SaveAsync()
    {
        var saved = context.SaveChanges();
        return saved > 0 ? true : false;
    }
}
