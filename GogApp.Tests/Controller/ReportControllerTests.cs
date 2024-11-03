using System;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GogApp.Controllers;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;

namespace GogApp.Tests.Controllers
{
    public class ReportControllerTests
    {
        private readonly IReportRepository _fakeReportRepo;
        private readonly ReportController _controller;

        public ReportControllerTests()
        {
            _fakeReportRepo = A.Fake<IReportRepository>();
            _controller = new ReportController(_fakeReportRepo);
            // Initialize TempData as a mock TempDataDictionary
            _controller.TempData = new TempDataDictionary(new DefaultHttpContext(), A.Fake<ITempDataProvider>());
        }

        [Fact]
        public async Task Create_ValidReport_RedirectsToProjectDetailWithSuccessMessage()
        {
            // Arrange
            var projectId = 1;
            var content = "Test content";
            var title = "Test title";

            // Act
            var result = await _controller.Create(projectId, content, title);

            // Assert
            A.CallTo(() => _fakeReportRepo.Add(A<Report>.Ignored)).MustHaveHappenedOnceExactly();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirectResult.ActionName.Should().Be("Detail");
            redirectResult.ControllerName.Should().Be("Project");
            _controller.TempData["SuccessMessage"].Should().BeEquivalentTo("Report added successfully.");
        }

        [Fact]
        public async Task Create_InvalidReportContent_ReturnsToProjectDetailsWithErrorMessage()
        {
            // Arrange
            var projectId = 1;
            var content = "";  // Invalid content
            var title = "Test title";

            // Act
            var result = await _controller.Create(projectId, content, title);

            // Assert
            A.CallTo(() => _fakeReportRepo.Add(A<Report>.Ignored)).MustNotHaveHappened();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirectResult.ActionName.Should().Be("Details");
            redirectResult.ControllerName.Should().Be("Project");
            _controller.TempData["ErrorMessage"].Should().Be("Report content cannot be empty.");
        }

        [Fact]
        public async Task Edit_GetValidReport_ReturnsEditViewWithModel()
        {
            // Arrange
            var reportId = 1;
            var report = new Report
            {
                Id = reportId,
                Content = "Existing content",
                ProjectId = 1,
                CreatedAt = DateTime.Now
            };

            A.CallTo(() => _fakeReportRepo.GetReportByIdAsyncNoTracking(reportId)).Returns(report);

            // Act
            var result = await _controller.Edit(reportId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<EditReportViewModel>().Which;
            model.Id.Should().Be(report.Id);
            model.Content.Should().Be(report.Content);
            model.ProjectId.Should().Be(report.ProjectId);
        }

        [Fact]
        public async Task Edit_GetInvalidReport_ReturnsNotFound()
        {
            // Arrange
            var reportId = 999;  // Assume this ID doesn't exist
            A.CallTo(() => _fakeReportRepo.GetReportByIdAsyncNoTracking(reportId)).Returns(Task.FromResult<Report>(null));

            // Act
            var result = await _controller.Edit(reportId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_PostValidModel_UpdatesReportAndRedirects()
        {
            // Arrange
            var reportId = 1;
            var model = new EditReportViewModel
            {
                Id = reportId,
                Content = "Updated content",
                ProjectId = 1
            };

            var report = new Report
            {
                Id = reportId,
                Content = "Existing content",
                ProjectId = model.ProjectId,
                CreatedAt = DateTime.Now
            };

            A.CallTo(() => _fakeReportRepo.GetReportByIdAsyncNoTracking(reportId)).Returns(report);

            // Act
            var result = await _controller.Edit(model);

            // Assert
            A.CallTo(() => _fakeReportRepo.Update(A<Report>.Ignored)).MustHaveHappenedOnceExactly();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirectResult.ActionName.Should().Be("Detail");
            redirectResult.ControllerName.Should().Be("Project");
            redirectResult.RouteValues["id"].Should().Be(model.ProjectId);
            _controller.TempData["SuccessMessage"].Should().Be("Report updated successfully.");
        }

        [Fact]
        public async Task Edit_PostInvalidModel_ReturnsEditViewWithModel()
        {
            // Arrange
            var model = new EditReportViewModel { Id = 1, Content = "", ProjectId = 1 };  // Invalid content
            _controller.ModelState.AddModelError("Content", "Content is required");

            // Act
            var result = await _controller.Edit(model);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().Be(model);  // Returns the same model with validation errors
        }

        [Fact]
        public async Task Delete_GetValidReport_ReturnsDeleteViewWithModel()
        {
            // Arrange
            var reportId = 1;
            var report = new Report { Id = reportId, Content = "Content to delete", ProjectId = 1 };

            A.CallTo(() => _fakeReportRepo.GetReportByIdAsync(reportId)).Returns(report);

            // Act
            var result = await _controller.Delete(reportId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().Be(report);
        }

        [Fact]
        public async Task Delete_GetInvalidReport_ReturnsNotFound()
        {
            // Arrange
            var reportId = 999;  // Assume this ID doesn't exist
            A.CallTo(() => _fakeReportRepo.GetReportByIdAsync(reportId)).Returns(Task.FromResult<Report>(null));

            // Act
            var result = await _controller.Delete(reportId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteConfirmed_ValidReport_DeletesReportAndRedirects()
        {
            // Arrange
            var reportId = 1;
            var report = new Report { Id = reportId, ProjectId = 1 };

            A.CallTo(() => _fakeReportRepo.GetReportByIdAsync(reportId)).Returns(report);

            // Act
            var result = await _controller.DeleteConfirmed(reportId);

            // Assert
            A.CallTo(() => _fakeReportRepo.Delete(report)).MustHaveHappenedOnceExactly();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirectResult.ActionName.Should().Be("Detail");
            redirectResult.ControllerName.Should().Be("Project");
            redirectResult.RouteValues["id"].Should().Be(report.ProjectId);
            _controller.TempData["SuccessMessage"].Should().Be("Report deleted successfully.");
        }
    }
}
