using System;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class ProjectTaskController : Controller
{
    private readonly IProjectTaskRepository taskRepo;
    private readonly ITaskVolunteerRepository taskVolunteerRepo;

    public ProjectTaskController(IProjectTaskRepository taskRepository, ITaskVolunteerRepository taskVolunteerRepository)
    {
        taskRepo = taskRepository;
        taskVolunteerRepo = taskVolunteerRepository;
    }

    [HttpGet]
    public IActionResult AddTask(int projectId)
    {
        var createProjectTaskViewModel = new CreateProjectTaskViewModel
        {
            ProjectId = projectId
        };
        return View(createProjectTaskViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddTask(CreateProjectTaskViewModel taskVM)
    {
        if (!ModelState.IsValid)
        {
            // Return the view with validation errors if the model state is invalid
            return View(taskVM);
        }

        try
        {
            var task = new ProjectTask
            {
                Title = taskVM.Title,
                ProjectId = taskVM.ProjectId,
                AssignedAt = taskVM.AssignedAt,
                CompletedAt = taskVM.CompletedAt
            };

            // Try to add the task asynchronously
            taskRepo.Add(task);

            // Redirect to the project details page on success
            return RedirectToAction("Detail", "Project", new { id = taskVM.ProjectId });
        }
        catch (Exception ex)
        {
            //A generic error message to ModelState so it's displayed in the view
            ModelState.AddModelError(string.Empty, "An error occurred while adding the task. Please try again later.");

            // Return the view with the taskVM so the user can retry
            return View(taskVM);
        }


    }


    public async Task<IActionResult> TaskDetails(int Id)
    {
        var task = await taskRepo.GetTaskByIdAsync(Id);
        if (task == null)
        {
            return NotFound();
        }

        var assignedTaskVolunteers = await taskVolunteerRepo.GetTaskVolunteersByTaskIdAsync(Id);

        var taskDetailViewModel = new TaskDetailViewModel
        {
            Task = task,
            AssignedVolunteers = assignedTaskVolunteers
        };

        return View(taskDetailViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int Id)
    {
        var projectTaskDetails = await taskRepo.GetTaskByIdAsync(Id);
        if (projectTaskDetails == null) return View("Error");
        return View(projectTaskDetails);
    }

    // Delete ProjectTask (cascading delete will handle TaskVolunteers)
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteTask(int Id)
    {
        var projectTask = await taskRepo.GetTaskByIdAsync(Id);

        if (projectTask == null)
        {
            return NotFound();
        }
        var projectId = projectTask.ProjectId;
        // Delete the project task
        taskRepo.Delete(projectTask);

        return RedirectToAction("Detail", "Project", new { id = projectId }); // or any other relevant view
    }
}