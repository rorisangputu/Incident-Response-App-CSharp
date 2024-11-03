using System.Net.Http.Json;
using System.Threading.Tasks;
using GogApp.Controllers;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace GogApp.Tests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AccountControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Register_ValidModel_CreatesUserAndRedirectsToHome()
        {
            // Arrange
            var client = _factory.CreateClient();
            var registerModel = new RegisterViewModel
            {
                EmailAddress = "test@example.com",
                Name = "testuser",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/Account/Register", registerModel);
            response.EnsureSuccessStatusCode(); // Ensure the response was successful

            // Assert
            Assert.Equal("/Home/Index", response.RequestMessage.RequestUri.AbsolutePath);
        }

        [Fact]
        public async Task Register_ExistingEmail_ReturnsViewWithError()
        {
            // Arrange
            var client = _factory.CreateClient();
            var registerModel = new RegisterViewModel
            {
                EmailAddress = "existing@example.com",
                Name = "existinguser",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // First, register the user to ensure it exists
            await client.PostAsJsonAsync("/Account/Register", registerModel);

            // Act
            var response = await client.PostAsJsonAsync("/Account/Register", registerModel);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.False(response.IsSuccessStatusCode); // Should not be successful
            Assert.Contains("This email address is already in use", content); // Check error message
        }

        [Fact]
        public async Task Login_ValidCredentials_RedirectsToHome()
        {
            // Arrange
            var client = _factory.CreateClient();
            var registerModel = new RegisterViewModel
            {
                EmailAddress = "login@example.com",
                Name = "loginuser",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            // First, register the user to ensure they can log in
            await client.PostAsJsonAsync("/Account/Register", registerModel);

            var loginModel = new LoginViewModel
            {
                EmailAddress = "login@example.com",
                Password = "Password123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/Account/Login", loginModel);
            response.EnsureSuccessStatusCode(); // Ensure the response was successful

            // Assert
            Assert.Equal("/Home/Index", response.RequestMessage.RequestUri.AbsolutePath);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginModel = new LoginViewModel
            {
                EmailAddress = "nonexistent@example.com",
                Password = "WrongPassword!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/Account/Login", loginModel);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.False(response.IsSuccessStatusCode); // Should not be successful
            Assert.Contains("Wrong credentials. Please Try again", content); // Check error message
        }
    }
}
