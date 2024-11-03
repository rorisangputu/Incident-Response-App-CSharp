using System;
using GogApp.Controllers;
using GogApp.Interfaces;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using FakeItEasy;
using GogApp.Models;
using Microsoft.AspNetCore.Mvc;
using GogApp.ViewModels;


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

    [Fact]
    public async Task ProjectController_Detail_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        int nonExistentId = 1;
        A.CallTo(() => projectRepo.GetByIdAsync(nonExistentId)).Returns(Task.FromResult<Project>(null));

        // Act
        var result = await projectController.Detail(nonExistentId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task ProjectController_Detail_ReturnsViewWithProjectDetailViewModel_WhenProjectExists()
    {
        // Arrange
        int existingProjectId = 1;
        var project = A.Fake<Project>();
        var volunteers = A.Fake<IEnumerable<ProjectVolunteer>>();
        A.CallTo(() => projectRepo.GetByIdAsync(existingProjectId)).Returns(project);
        A.CallTo(() => projectVolunteerRepo.GetAllProjectVolunteersAsync(existingProjectId)).Returns(volunteers);

        // Act
        var result = await projectController.Detail(existingProjectId);

        // Assert
        result.Should().BeOfType<ViewResult>().Which.Model.Should().BeOfType<ProjectDetailViewModel>();
        var model = (result as ViewResult)?.Model as ProjectDetailViewModel;
        model.Should().NotBeNull();
        model.Project.Should().BeEquivalentTo(project);
        model.Volunteers.Should().BeEquivalentTo(volunteers);
    }

    //CREATE
    [Fact]
    public async Task ProjectController_Create_Post_ReturnsRedirectToIndex()
    {
        // Arrange
        var createProjectViewModel = new CreateProjectViewModel
        {
            Title = "New Project",
            ManagerId = "testUserId",
            Description = "Project Description",
            Details = "Project Details"
        };

        // Act
        var result = await projectController.Create(createProjectViewModel);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        A.CallTo(() => projectRepo.Add(A<Project>._)).Invokes(() => { });
    }

    [Fact]
    public async Task ProjectController_Create_Post_ReturnsViewWithModel_WhenModelStateIsInvalid()
    {
        // Arrange
        var createProjectViewModel = new CreateProjectViewModel();
        projectController.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await projectController.Create(createProjectViewModel);

        // Assert
        result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(createProjectViewModel);
    }

    //DELETE
    [Fact]
    public async Task ProjectController_Delete_ReturnsViewWithProject_WhenProjectExists()
    {
        // Arrange
        int existingProjectId = 1;
        var project = A.Fake<Project>();
        A.CallTo(() => projectRepo.GetByIdAsync(existingProjectId)).Returns(project);

        // Act
        var result = await projectController.Delete(existingProjectId);

        // Assert
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(project);
    }

    [Fact]
    public async Task ProjectController_Delete_ReturnsErrorView_WhenProjectDoesNotExist()
    {
        // Arrange
        int nonExistingProjectId = 999;
        A.CallTo(() => projectRepo.GetByIdAsync(nonExistingProjectId)).Returns(Task.FromResult<Project>(null));

        // Act
        var result = await projectController.Delete(nonExistingProjectId);

        // Assert
        result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Error");
    }

    [Fact]
    public async Task ProjectController_DeleteProject_ReturnsRedirectToIndex_WhenProjectExists()
    {
        // Arrange
        int existingProjectId = 1;
        var project = A.Fake<Project>();
        A.CallTo(() => projectRepo.GetByIdAsync(existingProjectId)).Returns(project);

        // Act
        var result = await projectController.DeleteProject(existingProjectId);

        // Assert
        result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        A.CallTo(() => projectRepo.Delete(project)).MustHaveHappened();
    }

    [Fact]
    public async Task ProjectController_DeleteProject_ReturnsErrorView_WhenProjectDoesNotExist()
    {
        // Arrange
        int nonExistingProjectId = 999;
        A.CallTo(() => projectRepo.GetByIdAsync(nonExistingProjectId)).Returns(Task.FromResult<Project>(null));

        // Act
        var result = await projectController.DeleteProject(nonExistingProjectId);

        // Assert
        result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Error");
    }
}


