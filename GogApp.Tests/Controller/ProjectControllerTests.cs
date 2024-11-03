using System;
using GogApp.Controllers;
using GogApp.Interfaces;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using FakeItEasy;
using GogApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace GogApp.Tests.Controller;

public class ProjectControllerTests
{
    private ProjectController projectController;
    private IProjectRepository projectRepo;
    private IProjectVolunteerRepository projectVolunteerRepo;
    private IHttpContextAccessor contextAccessor;

    public ProjectControllerTests()
    {
        projectRepo = A.Fake<IProjectRepository>();
        projectVolunteerRepo = A.Fake<IProjectVolunteerRepository>();
        contextAccessor = A.Fake<HttpContextAccessor>();

        //SUT
        projectController = new ProjectController(projectRepo, projectVolunteerRepo, contextAccessor);
    }

    [Fact]
    public void PorjectController_Index_ReturnsSuccess()
    {
        var project = A.Fake<IEnumerable<Project>>();
        A.CallTo(() => projectRepo.GetAll()).Returns(project);

        var result = projectController.Index();

        result.Should().BeOfType<Task<IActionResult>>();
    }
}
