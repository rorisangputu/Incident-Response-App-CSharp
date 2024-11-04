using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace MyApp.Tests
{
    [TestClass]
    public class ProjectControllerTests
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
        public void TestLogin()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Login");

            // Fill in the login form
            driver.FindElement(By.Id("Email")).SendKeys("testuser@example.com");
            driver.FindElement(By.Id("Password")).SendKeys("Test@1234");

            // Click the login button
            driver.FindElement(By.Id("login-button")).Click();

            // Wait for navigation and assert successful login
            Thread.Sleep(2000); // Adjust as necessary
            //Assert.IsTrue(driver.PageSource.Contains("Welcome")); // Change the message accordingly
        }

        [TestMethod]
        public void TestCreateProject()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Login");

            // Fill in the login form
            driver.FindElement(By.Id("Email")).SendKeys("testuser@example.com");
            driver.FindElement(By.Id("Password")).SendKeys("Test@1234");

            // Click the login button
            driver.FindElement(By.Id("login-button")).Click();

            // Wait for navigation and assert successful login
            Thread.Sleep(2000); // Adjust as necessary
            // Navigate to the Create Project page
            driver.Navigate().GoToUrl($"{baseUrl}/Project/Create");

            // Fill in the project form
            driver.FindElement(By.Name("Title")).SendKeys("New Test Project");
            driver.FindElement(By.Name("Description")).SendKeys("This is a description for the new test project.");
            driver.FindElement(By.Name("Details")).SendKeys("Additional details about the new test project.");

            // Click the Create button
            driver.FindElement(By.CssSelector("button[type='submit']")).Click();

            // Wait for navigation and assert success message or redirection
            Thread.Sleep(2000); // Adjust as necessary
            //Assert.IsTrue(driver.PageSource.Contains("Project created successfully")); // Adjust according to your success message
        }

        [TestMethod]
        public void TestProjectList()
        {
            // Navigate to the Project Index page
            driver.Navigate().GoToUrl($"{baseUrl}/Project");

            // Wait for the projects to load
            Thread.Sleep(2000); // Adjust as necessary

            // Assert that the project list is displayed
            Assert.IsTrue(driver.PageSource.Contains("Projects")); // Adjust based on your page content
            Assert.IsTrue(driver.PageSource.Contains("New Test Project")); // Ensure the newly created project is listed
        }

        [TestCleanup]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
