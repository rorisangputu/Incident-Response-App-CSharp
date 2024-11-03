using System;
using System.Threading.Tasks;
using FluentAssertions;
using GogApp.Data;
using GogApp.Models;
using GogApp.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GogApp.Tests.Repository
{
    public class ReportRepositoryTests
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
        public async Task ReportRepository_Add_ShouldAddReport()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var reportRepository = new ReportRepository(dbContext);
            var report = new Report
            {
                Title = "Test Report",
                Content = "Report Content",
                CreatedAt = DateTime.UtcNow,
                ProjectId = 1 // Ensure this ProjectId exists in the database if necessary
            };

            // Act
            var result = await reportRepository.Add(report);

            // Assert
            result.Should().BeTrue();
            dbContext.Reports.Should().Contain(r => r.Title == "Test Report");
        }

        [Fact]
        public async Task ReportRepository_GetReportByIdAsync_ShouldReturnCorrectReport()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var reportRepository = new ReportRepository(dbContext);
            var report = new Report
            {
                Title = "Test Report",
                Content = "Report Content",
                CreatedAt = DateTime.UtcNow,
                ProjectId = 1
            };
            await reportRepository.Add(report);

            // Act
            var retrievedReport = await reportRepository.GetReportByIdAsync(report.Id);

            // Assert
            retrievedReport.Should().NotBeNull();
            retrievedReport.Title.Should().Be("Test Report");
        }

        [Fact]
        public async Task ReportRepository_Update_ShouldModifyReport()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var reportRepository = new ReportRepository(dbContext);
            var report = new Report
            {
                Title = "Initial Title",
                Content = "Initial Content",
                CreatedAt = DateTime.UtcNow,
                ProjectId = 1
            };
            await reportRepository.Add(report);

            // Act
            report.Title = "Updated Title";
            await reportRepository.Update(report);
            var updatedReport = await reportRepository.GetReportByIdAsync(report.Id);

            // Assert
            updatedReport.Should().NotBeNull();
            updatedReport.Title.Should().Be("Updated Title");
        }

        [Fact]
        public async Task ReportRepository_Delete_ShouldRemoveReport()
        {
            // Arrange
            var dbContext = await GetDbContext();
            var reportRepository = new ReportRepository(dbContext);
            var report = new Report
            {
                Title = "Report to Delete",
                Content = "Content",
                CreatedAt = DateTime.UtcNow,
                ProjectId = 1
            };
            await reportRepository.Add(report);

            // Act
            var result = await reportRepository.Delete(report);

            // Assert
            //result.Should().BeTrue();
            dbContext.Reports.Should().BeEmpty();
        }
    }
}
