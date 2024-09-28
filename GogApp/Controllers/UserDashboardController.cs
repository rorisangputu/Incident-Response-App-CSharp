using System;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GogApp.Controllers;

public class UserDashboardController : Controller
{
    private readonly IUserDashboardRepository userDashboardRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public UserDashboardController(IUserDashboardRepository userDashboardRepository, IHttpContextAccessor httpContextAccessor,
    UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        userDashboardRepo = userDashboardRepository;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    public async Task<IActionResult> Index()
    {
        // Get the current user
        var userId = _httpContextAccessor?.HttpContext?.User.GetUserId();
        var user = await userDashboardRepo.GetUserWithProjectsAndTasksAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        // Populate the ViewModel
        var userDashboardViewModel = new UserDashboardViewModel
        {
            User = user,
            MyProjects = user.MyProjects,
            VolunteeredProjects = user?.ProjectVolunteers?.Select(pv => pv.Project).ToList(),
            AssignedTasks = user?.TaskVolunteers
        };

        return View(userDashboardViewModel);
    }

    [HttpGet]
    public async Task<IActionResult> EditUserProfile()
    {
        var curUserId = _httpContextAccessor?.HttpContext?.User.GetUserId();
        var user = await userDashboardRepo.GetUserById(curUserId);
        if (user == null) return View("Error");

        var editUserProfileViewModel = new EditUserProfileViewModel
        {
            UserName = user.UserName
        };

        return View(editUserProfileViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> EditUserProfile(EditUserProfileViewModel editUserProfileVM)
    {
        if (!ModelState.IsValid) return View(editUserProfileVM);

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound("User Not Found");

        user.UserName = editUserProfileVM.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(editUserProfileVM);
        }

        await _signInManager.RefreshSignInAsync(user); // Ensure user remains logged in
        TempData["SuccessMessage"] = "Profile updated successfully";
        return RedirectToAction("Index", "UserDashboard");
    }
}