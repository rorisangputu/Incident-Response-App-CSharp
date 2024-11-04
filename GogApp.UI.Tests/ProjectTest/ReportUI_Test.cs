using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace MyApp.Tests
{
    [TestClass]
    public class ReportUI_Test
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
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Login"); // Update to your actual login URL

            // Fill in the login form
            driver.FindElement(By.Id("Email")).SendKeys("testuser@example.com"); // Update with a valid test email
            driver.FindElement(By.Id("Password")).SendKeys("Test@1234"); // Update with a valid test password

            // Click the login button
            driver.FindElement(By.Id("login-button")).Click(); // Update with the actual login button ID

            // Wait for navigation and assert successful login
            Thread.Sleep(2000); // Adjust as necessary
            //Assert.IsTrue(driver.PageSource.Contains("Welcome")); // Adjust based on your actual welcome message
        }

        [TestMethod]
        public void TestCreateReport()
        {
            TestLogin(); // Ensure the user is logged in

            // Navigate to the Create Report page
            driver.Navigate().GoToUrl($"{baseUrl}/Project/Detail/5"); // Update to your actual create report URL

            // Fill in the report form
            driver.FindElement(By.Name("reportTitle")).SendKeys("Monthly Sales Report"); // Update the title field name if necessary
            driver.FindElement(By.Name("reportContent")).SendKeys("This report summarizes the sales for the month of October."); // Update if necessary

            // Click the Create button
            driver.FindElement(By.CssSelector(cssSelectorToFind: "button[type='submit']")).Click(); // Update if necessary

            // Wait for navigation and assert success message or redirection
            Thread.Sleep(2000); // Adjust as necessary
            //Assert.IsTrue(driver.PageSource.Contains("Report created successfully")); // Adjust according to your success message
        }



        [TestCleanup]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
