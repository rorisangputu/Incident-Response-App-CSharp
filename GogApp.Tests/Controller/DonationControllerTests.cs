using System;
using System.Security.Claims;
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
    public class DonationControllerTests
    {
        private readonly IDonationRepository _fakeDonationRepo;
        private readonly IProjectRepository _fakeProjectRepo;
        private readonly IHttpContextAccessor _fakeHttpContextAccessor;
        private readonly DonationController _controller;

        public DonationControllerTests()
        {
            _fakeDonationRepo = A.Fake<IDonationRepository>();
            _fakeProjectRepo = A.Fake<IProjectRepository>();
            _fakeHttpContextAccessor = A.Fake<IHttpContextAccessor>();

            // Set up HttpContext with a user having a specific user ID claim
            var httpContext = new DefaultHttpContext();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim(ClaimTypes.NameIdentifier, "TestUserId") // This is where we set the user ID
            }, "mock"));

            httpContext.User = user;
            _fakeHttpContextAccessor.HttpContext = httpContext;

            // Initialize controller and TempData
            _controller = new DonationController(_fakeDonationRepo, _fakeProjectRepo, _fakeHttpContextAccessor)
            {
                TempData = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>())
            };
        }

        [Fact]
        public async Task Create_Get_ReturnsViewWithCreateDonationViewModel()
        {
            // Arrange
            var projectId = 1;
            var project = new Project { Id = projectId, Title = "Test Project" };
            A.CallTo(() => _fakeProjectRepo.GetByIdAsync(projectId)).Returns(Task.FromResult(project));

            // Act
            var result = await _controller.Create(projectId);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<CreateDonationViewModel>().Which;
            model.ProjectId.Should().Be(projectId);
            model.ProjectName.Should().Be("Test Project");
            model.AppUserId.Should().Be("TestUserId");
        }

        [Fact]
        public async Task Create_Post_ValidModel_RedirectsToProjectDetail()
        {
            // Arrange
            var donationVM = new CreateDonationViewModel
            {
                ProjectId = 1,
                Item = "Test Item",
                Quantity = 5,
                AppUserId = "TestUserId"
            };

            // Act
            var result = await _controller.Create(donationVM);

            // Assert
            A.CallTo(() => _fakeDonationRepo.Add(A<Donation>.Ignored)).MustHaveHappenedOnceExactly();
            var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Which;
            redirectResult.ActionName.Should().Be("Detail");
            redirectResult.ControllerName.Should().Be("Project");
            redirectResult.RouteValues["id"].Should().Be(donationVM.ProjectId);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var donationVM = new CreateDonationViewModel();
            _controller.ModelState.AddModelError("Item", "Required");

            // Act
            var result = await _controller.Create(donationVM);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().BeEquivalentTo(donationVM);
        }

        [Fact]
        public async Task Create_Post_ExceptionOccurs_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var donationVM = new CreateDonationViewModel
            {
                ProjectId = 1,
                Item = "Test Item",
                Quantity = 5,
                AppUserId = "TestUserId"
            };
            A.CallTo(() => _fakeDonationRepo.Add(A<Donation>.Ignored)).Throws<Exception>();

            // Act
            var result = await _controller.Create(donationVM);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().BeEquivalentTo(donationVM);
            _controller.ModelState[string.Empty].Errors[0].ErrorMessage.Should().Be("An error occurred while adding the task. Please try again later.");
        }
    }
}
