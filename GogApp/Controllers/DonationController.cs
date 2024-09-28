using System;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class DonationController : Controller
{
    private readonly IDonationRepository donationRepo;
    private readonly IProjectRepository projectRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DonationController(IDonationRepository donationRepository, IProjectRepository projectRepository,
    IHttpContextAccessor httpContextAccessor)
    {
        donationRepo = donationRepository;
        _httpContextAccessor = httpContextAccessor;
        projectRepo = projectRepository;
    }

    public async Task<IActionResult> Create(int projectId)
    {
        var curUserId = _httpContextAccessor.HttpContext.User.GetUserId();
        var project = await projectRepo.GetByIdAsync(projectId);
        var createDonationViewModel = new CreateDonationViewModel
        {

            ProjectId = projectId,
            ProjectName = project.Title,
            AppUserId = curUserId
        };
        return View(createDonationViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDonationViewModel donationVM)
    {
        if (!ModelState.IsValid)
        {
            return View(donationVM);
        }

        try
        {
            var donation = new Donation
            {
                Item = donationVM.Item,
                Quantity = donationVM.Quantity,
                AppUserId = donationVM.AppUserId,
                ProjectId = donationVM.ProjectId,
                DonatedAt = DateTime.Now
            };

            await donationRepo.Add(donation);

            return RedirectToAction("Detail", "Project", new { id = donationVM.ProjectId });
        }
        catch (System.Exception)
        {
            //A generic error message to ModelState so it's displayed in the view
            ModelState.AddModelError(string.Empty, "An error occurred while adding the task. Please try again later.");

            // Return the view with the taskVM so the user can retry
            return View(donationVM);
        }
    }
}
