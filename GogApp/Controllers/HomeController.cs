using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GogApp.Models;
using GogApp.ViewModels;
using GogApp.Interfaces;

namespace GogApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IProjectRepository _projectRepository;
    public HomeController(ILogger<HomeController> logger, IProjectRepository projectRepository)
    {
        _logger = logger;
        _projectRepository = projectRepository;
    }

    public async Task<IActionResult> Index()
    {
        // Fetch the latest 3 projects from the repository
        var latestProjects = (await _projectRepository.GetAll())
                                .OrderByDescending(p => p.Id)
                                .Take(3)
                                .ToList();

        // Create the ViewModel and pass it to the view
        var viewModel = new HomePageViewModel
        {
            LatestProjects = latestProjects
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
