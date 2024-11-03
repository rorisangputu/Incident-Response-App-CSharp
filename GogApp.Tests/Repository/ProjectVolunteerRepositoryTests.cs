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
    public class ProjectVolunteerRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        [Fact]
        public async Task ProjectVolunteerRepository_AddVolunteerAsync_ShouldAddVolunteer()
        {
            // Arrange
            var dbContext = await GetDbContext();

            // Create and add a project to the context
            var project = new Project
            {
                Title = "Test Project",
                ManagerId = "manager123" // Set required properties
            };
            dbContext.Projects.Add(project);
            await dbContext.SaveChangesAsync(); // Ensure the project is saved and tracked

            // Retrieve the project from the context to ensure we're using the tracked instance
            var trackedProject = await dbContext.Projects.FindAsync(project.Id);

            var projectVolunteerRepo = new ProjectVolunteerRepository(dbContext);
            var volunteer = new ProjectVolunteer
            {
                Project = trackedProject, // Reference the existing tracked project
                Volunteer = new AppUser { Id = "user123" },
                SignedUpAt = DateTime.UtcNow // Set this property as needed
            };

            // Act
            var result = await projectVolunteerRepo.AddVolunteerAsync(volunteer);

            // Assert
            //result.Should().BeTrue();
            dbContext.ProjectVolunteers.Should().ContainSingle(v => v.Volunteer.Id == "user123");
        }



        [Fact]
        public async Task ProjectVolunteerRepository_GetAllProjectVolunteersAsync_ShouldReturnAllVolunteers()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var projectVolunteerRepo = new ProjectVolunteerRepository(dbContext);
            projectVolunteerRepo.AddVolunteerAsync(new ProjectVolunteer { Project = new Project { Id = 1 }, Volunteer = new AppUser { Id = "user123" } });
            projectVolunteerRepo.AddVolunteerAsync(new ProjectVolunteer { Project = new Project { Id = 2 }, Volunteer = new AppUser { Id = "user456" } });
            projectVolunteerRepo.SaveAsync();
            // Act
            var result = await projectVolunteerRepo.GetAllProjectVolunteersAsync(1);

            // Assert
            result.Should().HaveCount(1);
        }


    }
}
