using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
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
            new DriverManager().SetUpDriver(new ChromeConfig());

            var options = new ChromeOptions();
            driver = new ChromeDriver(options);

            // Увеличить окно браузера
            driver.Manage().Window.Maximize();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            // Явное ожидание
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void TearDown()
        {
            driver?.Quit();
            driver = null;
        }

        [Test]
        public void OpenGoogleThenSearchEpamAndSearchAI()
        {
            // 1. Открыть Google
            driver.Navigate().GoToUrl("https://www.google.com");

            // Можно добавить ожидание, например ожидание появления поля поиска Google
            wait.Until(ExpectedConditions.ElementIsVisible(By.Name("q")));

            // 2. Увеличить окно (если не сделали в SetUp)
            driver.Manage().Window.Maximize();

            // 3. Перейти на epam.com
            driver.Navigate().GoToUrl("https://www.epam.com");

            // 4. Найти кнопку поиска (иконку лупы) и кликнуть
            var searchIcon = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector(".search-icon.dark-icon.header-search__search-icon")));
            searchIcon.Click();

            // 5. Дождаться появления панели поиска
            var searchPanel = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".header-search__panel")));

            // 6. Найти поле ввода поиска внутри панели
            var searchInput = searchPanel.FindElement(By.Name("q"));

            // 7. Ввести "AI"
            new Actions(driver)
                .Click(searchInput)
                .Pause(TimeSpan.FromSeconds(1))
                .SendKeys("AI")
                .Perform();

            // 8. Найти кнопку поиска рядом с полем и кликнуть
            var inputHolder = wait.Until(ExpectedConditions.ElementIsVisible(
                By.CssSelector("div.search-results__input-holder")
            ));

            var findButton = inputHolder.FindElement(By.XPath("./following-sibling::button"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", findButton);
            wait.Until(ExpectedConditions.ElementToBeClickable(findButton)).Click();

            Assert.Pass("Search for 'AI' completed and results are visible.");
        }
    }
}
