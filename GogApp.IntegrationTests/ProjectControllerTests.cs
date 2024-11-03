using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using GogApp;
using GogApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using GogApp.Data;
using Microsoft.VisualStudio.TestPlatform.TestHost;

public class ProjectControllerTests : IClassFixture<WebApplicationFactory<Program>>
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
        // Add test data to the in-memory database
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
        // Act
        var response = await _client.GetAsync("/Project");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Project 1", content);
        Assert.Contains("Test Project 2", content);
    }

    [Fact]
    public async Task Detail_ReturnsViewResult_WithProjectDetails()
    {
        // Act
        var response = await _client.GetAsync("/Project/Detail/1");

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Project 1", content);
        Assert.Contains("Description for test project 1", content);
    }

    [Fact]
    public async Task Create_AddsProject_RedirectsToIndex()
    {
        // Arrange
        var newProject = new Project { Title = "New Project", Description = "New Project Description", ManagerId = "manager1" };
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("Title", newProject.Title),
            new KeyValuePair<string, string>("Description", newProject.Description),
            new KeyValuePair<string, string>("ManagerId", newProject.ManagerId)
        });

        // Act
        var response = await _client.PostAsync("/Project/Create", content);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var projectListResponse = await _client.GetAsync("/Project");
        var projectListContent = await projectListResponse.Content.ReadAsStringAsync();
        Assert.Contains("New Project", projectListContent);
    }

    [Fact]
    public async Task Delete_RemovesProject_RedirectsToIndex()
    {
        // Act: First delete the project
        var response = await _client.PostAsync("/Project/Delete/1", null);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 200-299
        var projectListResponse = await _client.GetAsync("/Project");
        var projectListContent = await projectListResponse.Content.ReadAsStringAsync();
        Assert.DoesNotContain("Test Project 1", projectListContent);
    }
}
