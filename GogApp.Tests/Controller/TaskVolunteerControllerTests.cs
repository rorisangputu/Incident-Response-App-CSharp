using System.Collections.Generic;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GogApp.Controllers;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GogApp.Tests.Controller
{
    public class TaskVolunteerControllerTests
    {
        private readonly TaskVolunteerController _controller;
        private readonly IProjectTaskRepository _projectTaskRepo;
        private readonly IProjectVolunteerRepository _projectVolunteerRepo;
        private readonly ITaskVolunteerRepository _taskVolunteerRepo;

        public TaskVolunteerControllerTests()
        {
            // Use a mocking framework like FakeItEasy, Moq, or NSubstitute for these repositories
            _projectTaskRepo = A.Fake<IProjectTaskRepository>();
            _projectVolunteerRepo = A.Fake<IProjectVolunteerRepository>();
            _taskVolunteerRepo = A.Fake<ITaskVolunteerRepository>();
            _controller = new TaskVolunteerController(_projectTaskRepo, _projectVolunteerRepo, _taskVolunteerRepo);
        }

        [Fact]
        public async Task Assign_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            int taskId = 1;
            A.CallTo(() => _projectTaskRepo.GetTaskByIdAsync(taskId)).Returns(Task.FromResult<ProjectTask>(null));

            // Act
            var result = await _controller.Assign(taskId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Assign_ReturnsViewResult_WithAssignTaskVolunteerViewModel_WhenTaskExists()
        {
            // Arrange
            int taskId = 1;
            var task = new ProjectTask { Id = taskId, ProjectId = 2 };
            // Create a list of AppUser instances to represent volunteers
            var volunteers = new List<ProjectVolunteer>
            {
                new ProjectVolunteer { Id = 1, Volunteer = new AppUser { UserName = "Volunteer One" } },
                new ProjectVolunteer { Id = 2, Volunteer = new AppUser { UserName = "Volunteer Two" } }
            };

            A.CallTo(() => _projectTaskRepo.GetTaskByIdAsync(taskId)).Returns(Task.FromResult(task));
            A.CallTo(() => _projectVolunteerRepo.GetAllProjectVolunteersAsync(task.ProjectId)).Returns(Task.FromResult<IEnumerable<ProjectVolunteer>>(volunteers));

            // Act
            var result = await _controller.Assign(taskId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().BeOfType<AssignTaskVolunteerViewModel>();
            var model = (AssignTaskVolunteerViewModel)viewResult.Model;
            model.ProjectTaskId.Should().Be(taskId);
            model.AvailableVolunteers.Should().BeEquivalentTo(volunteers);
            model.ProjectTask.Should().Be(task);
        }

        [Fact]
        public async Task Assign_Post_ReturnsView_WhenModelStateIsInvalid()
        {
            // Arrange
            var assignTaskVolunteerVM = new AssignTaskVolunteerViewModel
            {
                ProjectTaskId = 1,
                SelectedVolunteerIds = new List<string> { "volunteer1" }
            };

            _controller.ModelState.AddModelError("Error", "Model is invalid"); // Simulate invalid model state

            // Act
            var result = await _controller.Assign(assignTaskVolunteerVM);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(assignTaskVolunteerVM);
        }

        [Fact]
        public async Task Assign_Post_AddsVolunteersAndRedirects_WhenModelStateIsValid()
        {
            // Arrange
            var assignTaskVolunteerVM = new AssignTaskVolunteerViewModel
            {
                ProjectTaskId = 1,
                SelectedVolunteerIds = new List<string> { "volunteer1", "volunteer2" }
            };

            // Act
            var result = await _controller.Assign(assignTaskVolunteerVM);

            // Assert
            A.CallTo(() => _taskVolunteerRepo.AddTaskVolunteerAsync(A<TaskVolunteer>._)).MustHaveHappenedTwiceExactly(); // Verify volunteers were added
            A.CallTo(() => _taskVolunteerRepo.SaveAsync()).MustHaveHappenedOnceExactly(); // Verify save was called
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirectResult.ActionName.Should().Be("Index");
            redirectResult.ControllerName.Should().Be("Project");
        }
    }
}
