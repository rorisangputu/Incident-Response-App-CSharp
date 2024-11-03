using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using GogApp.Data;
using GogApp.Models;
using GogApp.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GogApp.Tests.Repository
{
    public class UserDashboardRepositoryTests
    {
        private async Task<ApplicationDbContext> GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options;

            var dbContext = new ApplicationDbContext(options);
            await dbContext.Database.EnsureCreatedAsync();
            return dbContext;
        }

        [Fact]
        public async Task UserDashboardRepository_GetUserById_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var repository = new UserDashboardRepository(dbContext);
            var userId = "user123";
            var user = new AppUser { Id = userId, UserName = "Test User" };
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetUserById(userId);

            // Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task UserDashboardRepository_GetUserWithProjectsAndTasksAsync_ShouldReturnUserWithProjectsAndTasks()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var repository = new UserDashboardRepository(dbContext);
            var userId = "user456";
            var user = new AppUser
            {
                Id = userId,
                UserName = "User With Projects",
                MyProjects = new List<Project>(),
                TaskVolunteers = new List<TaskVolunteer>()
            };
            await dbContext.Users.AddAsync(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = await repository.GetUserWithProjectsAndTasksAsync(userId);

            // Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public void UserDashboardRepository_Update_ShouldReturnTrue_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var dbContext = GetDbContext().Result;
            var repository = new UserDashboardRepository(dbContext);
            var user = new AppUser { Id = "user789", UserName = "Update User" };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            // Act
            user.UserName = "Updated User";
            var result = repository.Update(user);

            // Assert
            result.Should().BeTrue();
            dbContext.Users.Find("user789").UserName.Should().Be("Updated User");
        }

        [Fact]
        public void UserDashboardRepository_Update_ShouldReturnFalse_WhenUserUpdateFails()
        {
            // Arrange
            var dbContext = GetDbContext().Result;
            var repository = new UserDashboardRepository(dbContext);
            var user = new AppUser { Id = "user890", UserName = "Fail User" };
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            // Act
            user.UserName = null; // This will cause validation to fail if you have validation logic in place
            var result = repository.Update(user);

            // Assert
            result.Should().BeFalse();
        }
    }
}
