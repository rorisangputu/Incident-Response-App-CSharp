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
    public class DonationControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public DonationControllerTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_Get_ReturnsViewResult_WithCreateDonationViewModel()
        {
            // Arrange
            int projectId = 1; // Replace with a valid project ID based on your seeded data.

            // Act
            var response = await _client.GetAsync($"/Donation/Create?projectId={projectId}");

            // Assert
            response.EnsureSuccessStatusCode(); // Check for a 200 OK response
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Project Name", content); // Ensure the view contains project name
            Assert.Contains("Create Donation", content); // Ensure the view contains the title
        }

        [Fact]
        public async Task Create_Post_ValidDonation_RedirectsToProjectDetail()
        {
            // Arrange
            var donationVM = new CreateDonationViewModel
            {
                Item = "Sample Item",
                Quantity = 5,
                ProjectId = 1, // Replace with a valid project ID
                AppUserId = "TestUserId" // Replace with a valid user ID
            };

            var content = new StringContent(JsonConvert.SerializeObject(donationVM), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Donation/Create", content);

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode); // Check for redirect (302)
            var redirectUrl = response.Headers.Location.ToString();
            Assert.Contains("Detail", redirectUrl); // Ensure redirect goes to Project Detail
        }

        [Fact]
        public async Task Create_Post_InvalidDonation_ReturnsViewResult_WithModelError()
        {
            // Arrange
            var donationVM = new CreateDonationViewModel
            {
                Item = "", // Invalid Item
                Quantity = -1, // Invalid Quantity
                ProjectId = 1, // Replace with a valid project ID
                AppUserId = "TestUserId" // Replace with a valid user ID
            };

            var content = new StringContent(JsonConvert.SerializeObject(donationVM), System.Text.Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/Donation/Create", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode); // Should return the view
            var contentString = await response.Content.ReadAsStringAsync();
            Assert.Contains("An error occurred while adding the task.", contentString); // Check for error message
        }
    }
}
