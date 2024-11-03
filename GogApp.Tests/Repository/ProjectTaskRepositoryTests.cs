using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using GogApp.Data;
using GogApp.Models;
using GogApp.Repository;

namespace GogApp.Tests.Repository
{
    public class ProjectTaskRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        [Fact]
        public async Task ProjectTaskRepository_Add_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectTaskRepo = new ProjectTaskRepository(dbContext);
            var projectTask = new ProjectTask
            {
                Title = "Task 1",
                ProjectId = 1,
                AssignedAt = DateTime.Now
            };

            // Act
            var result = projectTaskRepo.Add(projectTask);

            // Assert
            result.Should().BeTrue();
            dbContext.Tasks.Should().ContainSingle(t => t.Title == "Task 1");
        }

        [Fact]
        public async Task ProjectTaskRepository_GetTaskByIdAsync_ReturnsTask()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectTaskRepo = new ProjectTaskRepository(dbContext);
            var projectTask = new ProjectTask
            {
                Title = "Task 1",
                ProjectId = 1
            };
            projectTaskRepo.Add(projectTask);

            // Act
            var result = projectTaskRepo.GetTaskByIdAsync(projectTask.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<ProjectTask>>();
            
        }

        [Fact]
        public async Task ProjectTaskRepository_Delete_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectTaskRepo = new ProjectTaskRepository(dbContext);
            var projectTask = new ProjectTask
            {
                Title = "Task to Delete",
                ProjectId = 1
            };
            projectTaskRepo.Add(projectTask);

            // Act
            var result = projectTaskRepo.Delete(projectTask);

            // Assert
            result.Should().BeTrue();
            dbContext.Tasks.Should().BeEmpty();
        }

        [Fact]
        public async Task ProjectTaskRepository_GetAllTasksAsync_ReturnsAllTasks()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectTaskRepo = new ProjectTaskRepository(dbContext);
            projectTaskRepo.Add(new ProjectTask { Title = "Task 1", ProjectId = 1 });
            projectTaskRepo.Add(new ProjectTask { Title = "Task 2", ProjectId = 1 });
            projectTaskRepo.Save(); // Ensure changes are saved

            // Act
            var result = projectTaskRepo.GetAllTasksAsync(); // Again, using .Result here

            // Assert
            result.Should().BeOfType<Task<ProjectTask>>();
        }

        [Fact]
        public async Task ProjectTaskRepository_GetTasksByProjectIdAsync_ReturnsTasksForProject()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectTaskRepo = new ProjectTaskRepository(dbContext);
            projectTaskRepo.Add(new ProjectTask { Title = "Task 1", ProjectId = 1 });
            projectTaskRepo.Add(new ProjectTask { Title = "Task 2", ProjectId = 1 });
            projectTaskRepo.Add(new ProjectTask { Title = "Task 3", ProjectId = 2 });

            // Act
            var result = await projectTaskRepo.GetTasksByProjectIdAsync(1);

            // Assert
            result.Should().HaveCount(2);
            result.Should().Contain(t => t.Title == "Task 1");
            result.Should().Contain(t => t.Title == "Task 2");
        }
    }
}
