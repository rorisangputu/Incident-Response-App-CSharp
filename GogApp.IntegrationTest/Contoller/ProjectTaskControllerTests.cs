using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using GogApp.Models;
using GogApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace GogApp.Tests
{
    public class ProjectTaskControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProjectTaskControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddTask_Get_ReturnsViewResult_WithCreateProjectTaskViewModel()
        {
            // Arrange
            int projectId = 1; // Replace with a valid project ID based on your seeded data.

            // Act
            var response = await _client.GetAsync($"/ProjectTask/AddTask?projectId={projectId}");

            // Assert
            response.EnsureSuccessStatusCode(); // Check for a 200 OK response
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Create Task", content); // Ensure the view contains the title
        }

        [Fact]
        public async Task AddTask_Post_ValidTask_RedirectsToProjectDetail()
        {
            // Arrange
            var taskVM = new CreateProjectTaskViewModel
            {
                Title = "Sample Task",
                ProjectId = 1, // Replace with a valid project ID
                AssignedAt = DateTime.Now,
                CompletedAt = DateTime.Now.AddDays(1) // Ensure this is in the future
            };

            var content = new StringContent(JsonConvert.SerializeObject(taskVM), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/ProjectTask/AddTask", content);

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode); // Check for redirect (302)
            var redirectUrl = response.Headers.Location.ToString();
            Assert.Contains("Detail", redirectUrl); // Ensure redirect goes to Project Detail
        }

        [Fact]
        public async Task AddTask_Post_InvalidTask_ReturnsViewResult_WithModelError()
        {
            // Arrange
            var taskVM = new CreateProjectTaskViewModel
            {
                Title = "", // Invalid Title
                ProjectId = 1, // Replace with a valid project ID
                AssignedAt = DateTime.Now,
                CompletedAt = DateTime.Now.AddDays(-1) // Invalid CompletedAt (must be in the future)
            };

            var content = new StringContent(JsonConvert.SerializeObject(taskVM), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/ProjectTask/AddTask", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Should return the view
            var contentString = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occurred while adding the task.", contentString); // Check for error message
        }

        [Fact]
        public async Task TaskDetails_ReturnsViewResult_WithTaskDetailViewModel()
        {
            // Arrange
            int taskId = 1; // Replace with a valid task ID based on your seeded data.

            // Act
            var response = await _client.GetAsync($"/ProjectTask/TaskDetails?Id={taskId}");

            // Assert
            response.EnsureSuccessStatusCode(); // Check for a 200 OK response
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Task Details", content); // Ensure the view contains task details
        }

        [Fact]
        public async Task Delete_Get_ReturnsViewResult_WithProjectTask()
        {
            // Arrange
            int taskId = 1; // Replace with a valid task ID based on your seeded data.

            // Act
            var response = await _client.GetAsync($"/ProjectTask/Delete?Id={taskId}");

            // Assert
            response.EnsureSuccessStatusCode(); // Check for a 200 OK response
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Confirm Deletion", content); // Ensure the view contains the confirmation text
        }

        [Fact]
        public async Task Delete_Post_ValidTask_RedirectsToProjectDetail()
        {
            // Arrange
            int taskId = 1; // Replace with a valid task ID based on your seeded data.

            var content = new StringContent("", System.Text.Encoding.UTF8, "application/json"); // Empty body for delete

            // Act
            var response = await _client.PostAsync($"/ProjectTask/Delete?Id={taskId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode); // Check for redirect (302)
            var redirectUrl = response.Headers.Location.ToString();
            Assert.Contains("Detail", redirectUrl); // Ensure redirect goes to Project Detail
        }
    }
}
