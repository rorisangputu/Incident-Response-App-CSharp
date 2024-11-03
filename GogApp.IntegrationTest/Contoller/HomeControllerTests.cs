using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GogApp.Controllers;
using GogApp.Interfaces;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace GogApp.Tests
{
    public class HomeControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public HomeControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfLatestProjects()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Home/Index");
            response.EnsureSuccessStatusCode(); // Ensure we got a successful response
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Project", content); // Check if content includes the word "Project"
            // You can add more specific assertions based on the expected project data in your database.
        }
    }
}
