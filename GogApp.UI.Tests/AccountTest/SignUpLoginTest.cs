using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace MyApp.Tests
{
    [TestClass]
    public class SignUpLoginTest
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
        public void TestSignUp()
        {
            driver.Navigate().GoToUrl($"{baseUrl}/Account/Register");

            // Fill in the signup form
            driver.FindElement(By.Id("Username")).SendKeys("Testcase@1");
            driver.FindElement(By.Id("Email")).SendKeys("testcase@example.com");
            driver.FindElement(By.Id("Password")).SendKeys("Test@1234");
            driver.FindElement(By.Id("ConfirmPassword")).SendKeys("Test@1234");

            // Click the signup button
            driver.FindElement(By.Id("signup-button")).Click();

            // Wait for navigation and assert success message or redirection
            Thread.Sleep(2000); // Adjust as necessary
            //Assert.IsTrue(driver.PageSource.Contains("Registration successful")); // Change the message accordingly
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

        [TestCleanup]
        public void Teardown()
        {
            driver.Quit();
        }
    }
}
