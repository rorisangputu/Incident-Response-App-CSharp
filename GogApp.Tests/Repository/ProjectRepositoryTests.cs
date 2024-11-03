using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GogApp.Data;
using GogApp.Models;
using GogApp.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GogApp.Tests.Repository
{
    public class ProjectRepositoryTests
    {
        private Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new ApplicationDbContext(options);
            databaseContext.Database.EnsureCreated();

            return Task.FromResult(databaseContext);
        }

        [Fact]
        public async Task ProjectRepository_Add_ReturnsTrue()
        {
            // Arrange
            var project = new Project()
            {
                Title = "Project 1",
                Description = "Description for project 1",
                Details = "Details for project 1",
                ManagerId = "user123" // Assuming there's a user with this ID
            };

            var dbContext = await GetDbContext();
            var projectRepo = new ProjectRepository(dbContext);

            // Act
            var result = projectRepo.Add(project);

            // Assert
            result.Should().BeTrue();
            dbContext.Projects.Should().ContainSingle(p => p.Title == project.Title);
        }

        [Fact]
        public async Task ProjectRepository_GetAll_ReturnsProjects()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectRepo = new ProjectRepository(dbContext);

            // Add test projects
            projectRepo.Add(new Project() { Title = "Project 1", Description = "Desc 1", ManagerId = "user123" });
            projectRepo.Add(new Project() { Title = "Project 2", Description = "Desc 2", ManagerId = "user123" });

            // Act
            var projects = await projectRepo.GetAll();

            // Assert
            projects.Should().HaveCount(2);
        }

        [Fact]
        public async Task ProjectRepository_GetByIdAsync_ReturnsProject()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectRepo = new ProjectRepository(dbContext);
            var project = new Project()
            {
                Title = "Project 1",
                Description = "Desc 1",
                ManagerId = "user123"
            };

            // Ensure that we await the Add method
            projectRepo.Add(project);

            // Act
            var result = projectRepo.GetByIdAsync(project.Id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Task<Project>>();
            
        }


        [Fact]
        public async Task ProjectRepository_Delete_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectRepo = new ProjectRepository(dbContext);
            var project = new Project() { Title = "Project 1", Description = "Desc 1", ManagerId = "user123" };
            projectRepo.Add(project);

            // Act
            var result = projectRepo.Delete(project);

            // Assert
            result.Should().BeTrue();
            dbContext.Projects.Should().BeEmpty();
        }

        [Fact]
        public async Task ProjectRepository_Update_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectRepo = new ProjectRepository(dbContext);
            var project = new Project() { Title = "Project 1", Description = "Desc 1", ManagerId = "user123" };
            projectRepo.Add(project);
            project.Description = "Updated description";

            // Act
            var result = projectRepo.Update(project);

            // Assert
            result.Should().BeTrue();
            dbContext.Projects.Should().ContainSingle(p => p.Description == "Updated description");
        }
    }
}
