using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DemoUITests.PageObjectModels
{
    class PayeePage
    {
        private readonly IWebDriver Driver;
        private const string PageUrl = "https://www.demo.bnz.co.nz/client/payees";
        private const string PageTitle = "Internet Banking";

        //locator
        public By PayeePageHeader = By.ClassName("CustomPage-heading");
        public By ImagePickerLocator = By.ClassName("image-picker-wrapper");
        public By PayeeSuccessMsgLocator = By.Id("notification");
        public By PayeeErrorTooltipLocator = By.ClassName("js-tooltip-text");

        //Elements
        public IWebElement RequiredFieldErrorMsg => Driver.FindElement(By.ClassName("js-tooltip-text"));
        public IWebElement GenericErrorMsg => Driver.FindElement(By.ClassName("error-header"));
        public IEnumerable<IWebElement> PayeeDefaultList => Driver.FindElements(By.ClassName("js-payee-name"));
        public IEnumerable<IWebElement> PayeeSortedList => Driver.FindElements(By.ClassName("js-payee-name"));

        //Actions
        public string PayeeSuccessMsg => Driver.FindElement(By.Id("notification")).Text;
        public string GetRequiredFieldErrorMsg => Driver.FindElement(By.ClassName("js-tooltip-text")).Text;
        public void ClickAddPayeeButton() => Driver.FindElement(By.CssSelector(".js-add-payee")).Click();
        public void ClickSortByName() => Driver.FindElement(By.ClassName("js-payee-name-column")).Click();
        public void EnterPayeeName(string payeeName) => Driver.FindElement(By.Id("ComboboxInput-apm-name")).SendKeys(payeeName);

        public void EnterApmBank(string apmBank) => Driver.FindElement(By.Name("apm-bank")).SendKeys(apmBank);
        public void EnterApmBranch(string apmBranch) => Driver.FindElement(By.Name("apm-branch")).SendKeys(apmBranch);

        public void EnterApmAccount(string apmAccount) => Driver.FindElement(By.Name("apm-account")).SendKeys(apmAccount);
        public void EnterApmSuffix(string apmSuffix) => Driver.FindElement(By.Name("apm-suffix")).SendKeys(apmSuffix);
        public void SubmitNewPayee() => Driver.FindElement(By.CssSelector(".js-submit")).Click();

        public PayeePage(IWebDriver driver)
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

        public List<string> GetPayeeListNames() 
        {
            List<string> payeeListNameText = new List<string>(Driver.FindElements(By.ClassName("js-payee-name")).Select(iw => iw.Text));
            return payeeListNameText;
        }

        public List<string> GetPayeeListAccounts()
        {
            List<string> payeeListAccountText = new List<string>(Driver.FindElements(By.ClassName("js-payee-account")).Select(iw => iw.Text));
            return payeeListAccountText;
        }

        public void WaitForElementToBeVisible(By locator, int waitForSeconds)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(waitForSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public void WaitForElementToDisappear(By locator, int waitForSeconds)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(waitForSeconds));
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(locator));
        }

        public void CheckListInAscOrderByDefault()
        {
            PayeeDefaultList.Should().BeInAscendingOrder(x => x.Text, "Payee list name should be in ascending order by default");
        }

        public void CheckListInDescOrder()
        {
            PayeeSortedList.Should().BeInDescendingOrder(x => x.Text, "Payee list name should be in sorted in descending order");
        }
    }
}
