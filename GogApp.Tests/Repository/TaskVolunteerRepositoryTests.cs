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
    public class TaskVolunteerRepositoryTests
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
        public async Task TaskVolunteerRepository_AddTaskVolunteerAsync_ShouldAddVolunteer()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var repository = new TaskVolunteerRepository(dbContext);
            var taskVolunteer = new TaskVolunteer
            {
                ProjectTaskId = 1, // Ensure this TaskId exists in your test setup
                AppUserId = "volunteer123"
            };

            // Act
            var result = await repository.AddTaskVolunteerAsync(taskVolunteer);

            // Assert
            //result.Should().BeTrue();
            dbContext.TaskVolunteers.Should().ContainSingle(tv => tv.AppUserId == "volunteer123");
        }

    }
}
