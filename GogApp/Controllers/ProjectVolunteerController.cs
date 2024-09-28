using System;
using GogApp.Interfaces;
using GogApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class ProjectVolunteerController : Controller
{
    private readonly IProjectVolunteerRepository projectVolunteerRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IProjectRepository projectRepo;
    private readonly IUserDashboardRepository userDashboardRepo;

    public ProjectVolunteerController(IProjectVolunteerRepository projectVolunteerRepository, IHttpContextAccessor httpContextAccessor,
    IProjectRepository projectRepository, IUserDashboardRepository userDashboardRepository)
    {
        projectVolunteerRepo = projectVolunteerRepository;
        _httpContextAccessor = httpContextAccessor;
        projectRepo = projectRepository;
        userDashboardRepo = userDashboardRepository;
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(int projectId)
    {
        var user = _httpContextAccessor.HttpContext.User.GetUserId(); ;
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }

        // Check if the user is already signed up for this project
        var existingVolunteer = await projectVolunteerRepo.GetVolunteerAsync(projectId, user);
        var project = await projectRepo.GetByIdAsync(projectId);
        var volunteer = await userDashboardRepo.GetUserById(user);

        if (existingVolunteer == null)
        {
            var projectVolunteer = new ProjectVolunteer
            {
                Project = project,
                Volunteer = volunteer,
                SignedUpAt = DateTime.Now
            };

            await projectVolunteerRepo.AddVolunteerAsync(projectVolunteer);
            await projectVolunteerRepo.SaveAsync();

            TempData["SuccessMessage"] = "You have successfully signed up for the project!";

        }
        else
        {
            TempData["SuccessMessage"] = "You have already signed up for this project.";
        }

        return RedirectToAction("Detail", "Project", new { id = projectId });
    }

}
