using System.Security.Claims;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using GogApp.Controllers;
using GogApp.Data;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace GogApp.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly UserManager<AppUser> _fakeUserManager;
        private readonly SignInManager<AppUser> _fakeSignInManager;
        private readonly ApplicationDbContext _fakeContext;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _fakeUserManager = A.Fake<UserManager<AppUser>>();
            _fakeSignInManager = A.Fake<SignInManager<AppUser>>();
            _fakeContext = A.Fake<ApplicationDbContext>();

            // Set up HttpContext for TempData
            var httpContext = new DefaultHttpContext();
            _controller = new AccountController(_fakeUserManager, _fakeSignInManager, _fakeContext)
            {
                TempData = new TempDataDictionary(httpContext, A.Fake<ITempDataProvider>())
            };
        }

        [Fact]
        public void Register_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Register();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Register_Post_WithInvalidModel_ReturnsViewResultWithModel()
        {
            // Arrange
            var registerVM = new RegisterViewModel();
            _controller.ModelState.AddModelError("Name", "Required");

            // Act
            var result = await _controller.Register(registerVM);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(registerVM);
        }

        [Fact]
        public async Task Register_Post_WithExistingEmail_ReturnsViewResultWithErrorMessage()
        {
            // Arrange
            var registerVM = new RegisterViewModel { EmailAddress = "test@example.com", Name = "TestUser", Password = "Password123!" };
            A.CallTo(() => _fakeUserManager.FindByEmailAsync(registerVM.EmailAddress)).Returns(new AppUser());

            // Act
            var result = await _controller.Register(registerVM);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            _controller.TempData["Error"].Should().Be("This email address is already in use");
            viewResult.Model.Should().Be(registerVM);
        }

        [Fact]
        public async Task Register_Post_SuccessfulRegistration_RedirectsToHomeIndex()
        {
            // Arrange
            var registerVM = new RegisterViewModel { EmailAddress = "newuser@example.com", Name = "NewUser", Password = "Password123!" };
            A.CallTo(() => _fakeUserManager.CreateAsync(A<AppUser>.Ignored, registerVM.Password)).Returns(IdentityResult.Success);

            // Act
            var result = await _controller.Register(registerVM);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public void Login_Get_ReturnsViewResult()
        {
            // Act
            var result = _controller.Login();

            // Assert
            result.Should().BeOfType<ViewResult>();
        }

        [Fact]
        public async Task Login_Post_WithInvalidModel_ReturnsViewResultWithModel()
        {
            // Arrange
            var loginVM = new LoginViewModel();
            _controller.ModelState.AddModelError("EmailAddress", "Required");

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            result.Should().BeOfType<ViewResult>().Which.Model.Should().Be(loginVM);
        }

        [Fact]
        public async Task Login_Post_WithInvalidCredentials_ReturnsViewResultWithErrorMessage()
        {
            // Arrange
            var loginVM = new LoginViewModel { EmailAddress = "test@example.com", Password = "wrongpassword" };
            A.CallTo(() => _fakeUserManager.FindByEmailAsync(loginVM.EmailAddress)).Returns(new AppUser());
            A.CallTo(() => _fakeSignInManager.PasswordSignInAsync(A<AppUser>.Ignored, loginVM.Password, false, false))
                .Returns(SignInResult.Failed);

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            var viewResult = result.Should().BeOfType<ViewResult>().Which;
            _controller.TempData["Error"].Should().Be("Wrong credentials. Please Try again");
            viewResult.Model.Should().Be(loginVM);
        }

        [Fact]
        public async Task Login_Post_SuccessfulLogin_RedirectsToHomeIndex()
        {
            // Arrange
            var loginVM = new LoginViewModel { EmailAddress = "test@example.com", Password = "Password123!" };
            var user = new AppUser();
            A.CallTo(() => _fakeUserManager.FindByEmailAsync(loginVM.EmailAddress)).Returns(user);
            A.CallTo(() => _fakeSignInManager.PasswordSignInAsync(user, loginVM.Password, false, false)).Returns(SignInResult.Success);

            // Set up claims principal in HttpContext
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "TestUserId") }));
            _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = await _controller.Login(loginVM);

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
        }

        [Fact]
        public async Task Logout_RedirectsToHomeIndex()
        {
            // Act
            var result = await _controller.Logout();

            // Assert
            result.Should().BeOfType<RedirectToActionResult>().Which.ActionName.Should().Be("Index");
            A.CallTo(() => _fakeSignInManager.SignOutAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
