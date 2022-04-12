using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;


namespace DemoUITests.PageObjectModels
{
    class HomePage
    {
        private readonly IWebDriver Driver;
        private const string PageUrl = "https://www.demo.bnz.co.nz/client/";
        private const string PageTitle = "Internet Banking";
        private int waitForSeconds = 10;

        //Locator
        public By MenuIconLocator = By.ClassName("Icons--hamburguerMenu");
        public By PayeeLinkLocator = By.LinkText("Payees");
        public By PaymentLinkLocator = By.ClassName("IconPayTransfer");

        //Page Navigation Links
        public IWebElement MenuLink => Driver.FindElement(By.ClassName("Icons--hamburguerMenu"));
        public IWebElement PayeesLink => Driver.FindElement(By.LinkText("Payees"));
        public IWebElement PaymentLink => Driver.FindElement(By.ClassName("IconPayTransfer"));
        public HomePage(IWebDriver driver)
        {
            Driver = driver;
        }

        public void NavigateTo()
        {
            Driver.Navigate().GoToUrl(PageUrl);
            EnsurePageLoaded();
        }

        public void EnsurePageLoaded()
        {
            bool pageHasLoaded = (Driver.Url == PageUrl) && (Driver.Title == PageTitle);

            if (!pageHasLoaded)
            {
                throw new Exception(
                    $"Failed to load page. Page URL = '{Driver.Url}' Page source: \r\n {Driver.PageSource}");
            }
        }

        public PayeePage GoToPayeesPage()
        {
            WaitForElementToBeClickable(MenuIconLocator, waitForSeconds);
            MenuLink.Click();

            WaitForElementToBeClickable(PayeeLinkLocator, waitForSeconds);
            
            PayeesLink.Click();
            return new PayeePage(Driver);
        }

        public PaymentPage GoToPaymentsPage()
        {
            WaitForElementToBeClickable(MenuIconLocator, waitForSeconds);
            MenuLink.Click();

            WaitForElementToBeClickable(PaymentLinkLocator, waitForSeconds);

            PaymentLink.Click();
            return new PaymentPage(Driver);
        }

        public void WaitForElementToBeClickable(By locator, int waitForSeconds)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(waitForSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }
    }
}
