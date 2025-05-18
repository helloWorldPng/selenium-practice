using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;

namespace YourNamespace.Tests
{
    [TestFixture]
    public class EpamSearchTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        [Test, TestCaseSource(nameof(SearchTestCases))]
        public void TestSearchFunctionality(string url, string searchTerm)
        {
            driver.Navigate().GoToUrl(url);

            var searchIcon = wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("dark-iconheader-search__search-icon")));
            searchIcon.Click();

            var searchPanel = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("header-search__panel")));
            var searchInput = searchPanel.FindElement(By.Name("q"));

            new Actions(driver)
                .Click(searchInput)
                .Pause(TimeSpan.FromSeconds(1))
                .SendKeys(searchTerm)
                .Perform();

            var findButton = searchPanel.FindElement(By.XPath(".//*[@class='search-results__input-holder']/following-sibling::button"));
            findButton.Click();

            // Wait for results
            wait.Until(ExpectedConditions.ElementExists(By.ClassName("search-results__items")));
            Assert.Pass("Search results appeared successfully.");
        }

        static IEnumerable<TestCaseData> SearchTestCases()
        {
            yield return new TestCaseData("https://www.epam.com", "Automation").SetName("SearchForAutomation");
            yield return new TestCaseData("https://www.epam.com", "AI").SetName("SearchForAI");
        }
    }
}
