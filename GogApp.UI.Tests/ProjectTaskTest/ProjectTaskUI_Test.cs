using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace MyApp.Tests
{
    [TestClass]
    public class ProjectTaskUI_Test
    {
        private IWebDriver driver;
        private string baseUrl = "http://localhost:5014"; // Change to your application's URL

        [TestInitialize]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            driver.Manage().Window.Maximize();
        }

        [TestMethod]
        public void TestAddTask()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Login");

            // Fill in the login form
            driver.FindElement(By.Id("Email")).SendKeys("testuser@example.com");
            driver.FindElement(By.Id("Password")).SendKeys("Test@1234");

            // Click the login button
            driver.FindElement(By.Id("login-button")).Click();

            // Wait for navigation and assert successful login
            Thread.Sleep(2000); // Adjust as necessary
            // Navigate to the Create Task page for a specific project (use the actual project ID)
            int projectId = 8; // Replace with a valid project ID
            driver.Navigate().GoToUrl($"{baseUrl}/ProjectTask/AddTask?projectId={projectId}");

            // Fill in the task form
            driver.FindElement(By.Name("Title")).SendKeys("New Test Task");
            // Fill in other fields as necessary, e.g.:
            // driver.FindElement(By.Name("AssignedAt")).SendKeys(DateTime.Now.ToString("yyyy-MM-dd")); // Example for date input
            // driver.FindElement(By.Name("CompletedAt")).SendKeys(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd")); // Example for date input

            // Click the Add Task button
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Wait for navigation and assert success or that the task is listed in project details
            Thread.Sleep(2000); // Adjust as necessary
            //Assert.IsTrue(driver.PageSource.Contains("New Test Task")); // Check if the task title appears on the project details page
        }

        [TestMethod]
        public void TestTaskDetails()
        {
            // Navigate to the Task Details page (use a valid task ID)
            int taskId = 8; // Replace with a valid task ID
            driver.Navigate().GoToUrl($"{baseUrl}/ProjectTask/TaskDetails?id={taskId}");

            // Wait for the task details to load
            Thread.Sleep(2000); // Adjust as necessary

            // Assert that the task details are displayed correctly
            //Assert.IsTrue(driver.PageSource.Contains("New Test Task")); // Check if the task title appears on the task details page
            // Add more assertions as necessary to check other task details, like AssignedAt or CompletedAt
        }


        [TestMethod]
        public void TestAssignVolunteerToTask()
        {
            // Step 1: Navigate to the login page
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Login");

            // Step 2: Fill in the login form
            driver.FindElement(By.Id("Email")).SendKeys("testuser@example.com");
            driver.FindElement(By.Id("Password")).SendKeys("Test@1234");// Replace with valid password

            // Step 3: Click the login button
            driver.FindElement(By.Id("login-button")).Click();

            // Step 4: Wait for navigation and assert successful login
            Thread.Sleep(2000); // Adjust as necessary

            // Step 5: Navigate to the project details page
            int projectId = 8; // Replace with a valid project ID
            driver.Navigate().GoToUrl($"{baseUrl}/Project/Detail?id={projectId}");

            // Step 6: Click on the Assign Volunteer button for a specific task
            driver.Navigate().GoToUrl($"{baseUrl}/ProjectTask/TaskDetails/11");
            driver.FindElement(By.XPath("//a[contains(@href, 'TaskVolunteer/Assign/11')]")).Click();
            // Step 7: Select multiple volunteers from the dropdown
            var selectElement = driver.FindElement(By.Id("SelectedVolunteerIds"));
            var options = selectElement.FindElements(By.TagName("option"));
            options[0].Click(); // Select the first volunteer
            //options[1].Click(); // Select the second volunteer (hold Ctrl to select multiple)

            // Step 8: Click the Assign Volunteers button
            driver.FindElement(By.XPath("//button[text()='Assign Volunteers']")).Click();

            // Step 9: Wait for navigation and verify assignment
            Thread.Sleep(2000); // Adjust as necessary
           // Assert.IsTrue(driver.PageSource.Contains("Volunteers assigned successfully")); // Adjust according to confirmation message
        }

        [TestCleanup]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
