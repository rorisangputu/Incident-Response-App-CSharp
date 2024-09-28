using System;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class ProjectController : Controller
{
    private readonly IProjectRepository projectRepo;
    private readonly IProjectVolunteerRepository projectVolunteerRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    //Dependancy injection
    public ProjectController(IProjectRepository projectRepository, IProjectVolunteerRepository projectVolunteerRepository, IHttpContextAccessor httpContextAccessor)
    {
        projectRepo = projectRepository;
        projectVolunteerRepo = projectVolunteerRepository;
        _httpContextAccessor = httpContextAccessor;

    }

    public async Task<IActionResult> Index()
    {
        IEnumerable<Project> projects = await projectRepo.GetAll();
        return View(projects);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var project = await projectRepo.GetByIdAsync(id);

        if (project == null)
        {
            return NotFound();
        }

        // Fetch project volunteers for the specific project
        var volunteers = await projectVolunteerRepo.GetAllProjectVolunteersAsync(id);

        var projectDetailViewModel = new ProjectDetailViewModel
        {
            Project = project,
            Volunteers = volunteers
        };

        return View(projectDetailViewModel);
    }

    public IActionResult Create()
    {
        var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
        var createProjectViewModel = new CreateProjectViewModel { ManagerId = curUserId };
        return View(createProjectViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectViewModel projectVM)
    {
        if (ModelState.IsValid)
        {

            var project = new Project
            {
                Title = projectVM.Title,
                ManagerId = projectVM.ManagerId,
                Description = projectVM.Description,
                Details = projectVM.Details
            };

            // Add project to the repository or database
            projectRepo.Add(project);

            return RedirectToAction("Index");
        }
        else
        {
            // Add a model state error for display to the user
            ModelState.AddModelError(string.Empty, "An error occurred while creating the project. Please try again later.");
        }


        // If we reach this point, either ModelState is invalid or an exception occurred
        return View(projectVM);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var projectDetails = await projectRepo.GetByIdAsync(id);
        if (projectDetails == null) return View("Error");
        return View(projectDetails);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var projectDetails = await projectRepo.GetByIdAsync(id);
        if (projectDetails == null) return View("Error");

        projectRepo.Delete(projectDetails);
        return RedirectToAction("Index");
    }

    //Update

}
