using System;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class TaskVolunteerController : Controller
{
    private readonly IProjectTaskRepository projectTaskRepo;
    private readonly IProjectVolunteerRepository projectVolunteerRepo;
    private readonly ITaskVolunteerRepository taskVolunteerRepo;

    public TaskVolunteerController(IProjectTaskRepository projectTaskRepository, IProjectVolunteerRepository projectVolunteerRepository, ITaskVolunteerRepository taskVolunteerRepository)
    {
        projectTaskRepo = projectTaskRepository;
        projectVolunteerRepo = projectVolunteerRepository;
        taskVolunteerRepo = taskVolunteerRepository;

    }

    public async Task<IActionResult> Assign(int Id)
    {
        var task = await projectTaskRepo.GetTaskByIdAsync(Id);
        if (task == null)
        {
            NotFound();
        }

        var availableVolunteers = await projectVolunteerRepo.GetAllProjectVolunteersAsync(task.ProjectId);

        var assignTaskVolunteersViewModel = new AssignTaskVolunteerViewModel
        {
            ProjectTaskId = task.Id,
            AvailableVolunteers = availableVolunteers,
            ProjectTask = task
        };

        return View(assignTaskVolunteersViewModel);
    }
    //POST: Assign selected volunteers to a task
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Assign(AssignTaskVolunteerViewModel assignTaskVolunteerVM)
    {
        if (!ModelState.IsValid)
        {
            return View(assignTaskVolunteerVM);
        }

        // Loop through the selected volunteer IDs and assign them to the task
        foreach (var volunteerId in assignTaskVolunteerVM.SelectedVolunteerIds)
        {
            var taskVolunteer = new TaskVolunteer
            {
                ProjectTaskId = assignTaskVolunteerVM.ProjectTaskId,
                AppUserId = volunteerId
            };

            await taskVolunteerRepo.AddTaskVolunteerAsync(taskVolunteer); // Add the volunteer to the task
        }

        await taskVolunteerRepo.SaveAsync();

        return RedirectToAction("TaskDetails", "ProjectTask", new { TaskId = assignTaskVolunteerVM.ProjectTaskId }); // Redirect to the task details
    }
}
