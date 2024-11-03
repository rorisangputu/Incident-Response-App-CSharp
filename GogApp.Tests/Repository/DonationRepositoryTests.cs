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
    public class DonationRepositoryTests
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
        public async Task DonationRepository_Add_ReturnsTrue()
        {
            // Arrange
            var donation = new Donation()
            {
                Item = "Books",
                Quantity = 10,
                DonatedAt = DateTime.UtcNow,
                ProjectId = 1 // Assuming there's a project with ID 1
            };

            var dbContext = await GetDbContext();
            var donationRepo = new DonationRepository(dbContext);

            // Act
            var result = await donationRepo.Add(donation);

            // Assert
            result.Should().BeTrue();
            dbContext.Donations.Should().ContainSingle(d => d.Item == donation.Item);
        }

        [Fact]
        public async Task DonationRepository_GetDonationsByProjectId_ReturnsDonations()
        {
            // Arrange
            var projectId = 1;
            var dbContext = await GetDbContext();
            var donationRepo = new DonationRepository(dbContext);

            // Add test donations
            await donationRepo.Add(new Donation() { Item = "Books", Quantity = 10, DonatedAt = DateTime.UtcNow, ProjectId = projectId });
            await donationRepo.Add(new Donation() { Item = "Clothes", Quantity = 5, DonatedAt = DateTime.UtcNow, ProjectId = projectId });
            await donationRepo.Add(new Donation() { Item = "Toys", Quantity = 2, DonatedAt = DateTime.UtcNow, ProjectId = 2 }); // Different project

            // Act
            var donations = await donationRepo.GetDonationsByProjectId(projectId);

            // Assert
            donations.Should().HaveCount(2);
            donations.Should().OnlyContain(d => d.ProjectId == projectId);
        }
    }
}
