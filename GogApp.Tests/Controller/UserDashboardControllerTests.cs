using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GogApp.Controllers;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GogApp.Tests.Controller
{
    public class UserDashboardControllerTests
    {
        private readonly UserDashboardController dashboardController;
        private readonly IUserDashboardRepository userDashboardRepo;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;

        public UserDashboardControllerTests()
        {
            userDashboardRepo = A.Fake<IUserDashboardRepository>();
            httpContextAccessor = A.Fake<IHttpContextAccessor>();
            userManager = A.Fake<UserManager<AppUser>>();
            signInManager = A.Fake<SignInManager<AppUser>>();

            // System Under Test (SUT)
            dashboardController = new UserDashboardController(userDashboardRepo, httpContextAccessor, userManager, signInManager);
        }



        [Fact]
        public async Task Index_ReturnsViewWithUserDashboardViewModel_WhenUserExists()
        {
            // Arrange
            var userId = "user123";
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId), new Claim(ClaimTypes.Name, "TestUser") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            var appUser = new AppUser
            {
                UserName = "TestUser",
                About = "About me",
                MyProjects = new List<Project>(), // Add projects if needed
                ProjectVolunteers = new List<ProjectVolunteer>(),
                TaskVolunteers = new List<TaskVolunteer>()
            };

            A.CallTo(() => userDashboardRepo.GetUserWithProjectsAndTasksAsync(userId)).Returns(Task.FromResult(appUser));

            // Act
            var result = await dashboardController.Index();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            var model = viewResult.Model.Should().BeOfType<UserDashboardViewModel>().Which;

            model.User.Should().NotBeNull();
            model.User.UserName.Should().Be("TestUser"); // Verify the UserName
        }

        [Fact]
        public async Task EditUserProfile_ReturnsEditUserProfileViewModel_WhenUserExists()
        {
            // Arrange
            var userId = "user123";
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId), new Claim(ClaimTypes.Name, "TestUser") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuthType"));

            var httpContext = new DefaultHttpContext { User = claimsPrincipal };
            A.CallTo(() => httpContextAccessor.HttpContext).Returns(httpContext);

            var user = new AppUser
            {
                UserName = "TestUser",
                About = "About me"
            };

            A.CallTo(() => userDashboardRepo.GetUserById(userId)).Returns(Task.FromResult(user));

            // Act
            var result = await dashboardController.EditUserProfile();

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().BeOfType<EditUserProfileViewModel>();

            var model = (EditUserProfileViewModel)viewResult.Model;
            model.UserName.Should().Be(user.UserName);
            model.About.Should().Be(user.About);
        }


        [Fact]
        public async Task EditUserProfile_Post_ReturnsViewWithErrors_WhenModelStateIsInvalid()
        {
            // Arrange
            var editUserProfileVM = new EditUserProfileViewModel();
            dashboardController.ModelState.AddModelError("UserName", "Required");

            // Act
            var result = await dashboardController.EditUserProfile(editUserProfileVM);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(editUserProfileVM);
        }

        [Fact]
        public async Task EditUserProfile_Post_UpdatesUserAndRedirects_WhenModelIsValid()
        {
            // Arrange
            var editUserProfileVM = new EditUserProfileViewModel
            {
                UserName = "UpdatedUser",
                About = "Updated about"
            };
            var user = new AppUser { UserName = "OldUser", About = "Old about" };
            var updateResult = IdentityResult.Success;

            A.CallTo(() => userManager.GetUserAsync(dashboardController.User)).Returns(Task.FromResult(user));
            A.CallTo(() => userManager.UpdateAsync(user)).Returns(Task.FromResult(updateResult));
            A.CallTo(() => signInManager.RefreshSignInAsync(user)).Returns(Task.CompletedTask);

            // Act
            var result = await dashboardController.EditUserProfile(editUserProfileVM);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            user.UserName.Should().Be(editUserProfileVM.UserName);
            user.About.Should().Be(editUserProfileVM.About);
            dashboardController.TempData["SuccessMessage"].Should().Be("Profile updated successfully");
        }

        [Fact]
        public async Task EditUserProfile_Post_ReturnsViewWithErrors_WhenUpdateFails()
        {
            // Arrange
            var editUserProfileVM = new EditUserProfileViewModel
            {
                UserName = "UpdatedUser",
                About = "Updated about"
            };
            var user = new AppUser { UserName = "OldUser", About = "Old about" };
            var errors = new[] { new IdentityError { Description = "Update failed" } };

            A.CallTo(() => userManager.GetUserAsync(dashboardController.User)).Returns(Task.FromResult(user));
            A.CallTo(() => userManager.UpdateAsync(user)).Returns(Task.FromResult(IdentityResult.Failed(errors)));

            // Act
            var result = await dashboardController.EditUserProfile(editUserProfileVM);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            viewResult.Model.Should().Be(editUserProfileVM);
            dashboardController.ModelState.Should().Contain(error => error.Key == string.Empty);
        }
    }
}
