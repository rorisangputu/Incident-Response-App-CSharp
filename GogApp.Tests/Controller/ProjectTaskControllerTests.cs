using System;
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
    public class ProjectTaskControllerTests
    {
        private readonly ProjectTaskController taskController;
        private readonly IProjectTaskRepository taskRepo;
        private readonly ITaskVolunteerRepository taskVolunteerRepo;

        public ProjectTaskControllerTests()
        {
            taskRepo = A.Fake<IProjectTaskRepository>();
            taskVolunteerRepo = A.Fake<ITaskVolunteerRepository>();

            // System Under Test (SUT)
            taskController = new ProjectTaskController(taskRepo, taskVolunteerRepo);
        }

        [Fact]
        public void AddTask_ReturnsViewWithCreateProjectTaskViewModel()
        {
            // Arrange
            int projectId = 1;

            // Act
            var result = taskController.AddTask(projectId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CreateProjectTaskViewModel>().Subject;
            model.ProjectId.Should().Be(projectId);
        }

        [Fact]
        public async Task AddTask_Post_ReturnsRedirectToProjectDetail_WhenModelStateIsValid()
        {
            // Arrange
            var taskVM = new CreateProjectTaskViewModel
            {
                Title = "New Task",
                ProjectId = 1,
                AssignedAt = DateTime.Now,
                CompletedAt = DateTime.Now.AddDays(1)
            };

            // Act
            var result = await taskController.AddTask(taskVM);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Detail");
            A.CallTo(() => taskRepo.Add(A<ProjectTask>._)).MustHaveHappened();
        }

        [Fact]
        public async Task AddTask_Post_ReturnsViewWithModel_WhenModelStateIsInvalid()
        {
            // Arrange
            var taskVM = new CreateProjectTaskViewModel();
            taskController.ModelState.AddModelError("Title", "Required");

            // Act
            var result = await taskController.AddTask(taskVM);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(taskVM);
        }

        [Fact]
        public async Task TaskDetails_ReturnsViewWithTaskDetailViewModel_WhenTaskExists()
        {
            // Arrange
            int existingTaskId = 1;
            var task = A.Fake<ProjectTask>();
            var volunteers = A.Fake<IEnumerable<ProjectVolunteer>>();
            A.CallTo(() => taskRepo.GetTaskByIdAsync(existingTaskId)).Returns(task);
            // Act
            var result = await taskController.TaskDetails(existingTaskId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<TaskDetailViewModel>().Subject;
            model.Task.Should().Be(task);
            model.AssignedVolunteers.Should().BeEquivalentTo(volunteers);
        }

        [Fact]
        public async Task TaskDetails_ReturnsNotFound_WhenTaskDoesNotExist()
        {
            // Arrange
            int nonExistingTaskId = 999;
            A.CallTo(() => taskRepo.GetTaskByIdAsync(nonExistingTaskId)).Returns(Task.FromResult<ProjectTask>(null));

            // Act
            var result = await taskController.TaskDetails(nonExistingTaskId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_ReturnsViewWithProjectTask_WhenProjectTaskExists()
        {
            // Arrange
            int existingTaskId = 1;
            var projectTask = A.Fake<ProjectTask>();
            A.CallTo(() => taskRepo.GetTaskByIdAsync(existingTaskId)).Returns(projectTask);

            // Act
            var result = await taskController.Delete(existingTaskId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            viewResult.Model.Should().Be(projectTask);
        }

        [Fact]
        public async Task Delete_ReturnsErrorView_WhenProjectTaskDoesNotExist()
        {
            // Arrange
            int nonExistingTaskId = 999;
            A.CallTo(() => taskRepo.GetTaskByIdAsync(nonExistingTaskId)).Returns(Task.FromResult<ProjectTask>(null));

            // Act
            var result = await taskController.Delete(nonExistingTaskId);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.ViewName.Should().Be("Error");
        }

        [Fact]
        public async Task DeleteTask_ReturnsRedirectToProjectDetail_WhenProjectTaskExists()
        {
            // Arrange
            int existingTaskId = 1;
            
            var projectTask = A.Fake<ProjectTask>();
            A.CallTo(() => taskRepo.GetTaskByIdAsync(existingTaskId)).Returns(projectTask);

            // Act
            var result = await taskController.DeleteTask(existingTaskId);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Detail");
            A.CallTo(() => taskRepo.Delete(projectTask)).MustHaveHappened();
        }

        [Fact]
        public async Task DeleteTask_ReturnsNotFound_WhenProjectTaskDoesNotExist()
        {
            // Arrange
            int nonExistingTaskId = 999;
            A.CallTo(() => taskRepo.GetTaskByIdAsync(nonExistingTaskId)).Returns(Task.FromResult<ProjectTask>(null));

            // Act
            var result = await taskController.DeleteTask(nonExistingTaskId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
