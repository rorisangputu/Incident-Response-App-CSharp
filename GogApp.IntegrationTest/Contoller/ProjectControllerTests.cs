using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;
using GogApp; // Adjust this namespace according to your project's structure
using GogApp.Data;
using GogApp.Models;
using Microsoft.VisualStudio.TestPlatform.TestHost; // Ensure this is included for ApplicationDbContext

namespace GogApp.Tests
{
    public class ProjectControllerTests : IClassFixture<WebApplicationFactory<Program>> // Use Program instead of Startup
    {
        private readonly HttpClient _client;

        public ProjectControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add an in-memory database for testing
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDatabase");
                    });

                    // Recreate the database and seed with test data
                    var serviceProvider = services.BuildServiceProvider();
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                        db.Database.EnsureCreated();
                        SeedDatabase(db); // Seed the database with initial data
                    }
                });
            }).CreateClient();
        }

        private void SeedDatabase(ApplicationDbContext db)
        {
            // Add sample data to the in-memory database for testing
            db.Projects.AddRange(new[]
            {
                new Project { Id = 1, Title = "Test Project 1", Description = "Description for test project 1", ManagerId = "manager1" },
                new Project { Id = 2, Title = "Test Project 2", Description = "Description for test project 2", ManagerId = "manager2" }
            });
            db.SaveChanges();
        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithListOfProjects()
        {
            // Arrange
            var response = await _client.GetAsync("/Project");
            response.EnsureSuccessStatusCode();

            // Act
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Contains("Test Project 1", content);
            Assert.Contains("Test Project 2", content);
        }

        // Add more tests as necessary...
    }
}
