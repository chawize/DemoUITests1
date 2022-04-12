using System;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace DemoUITests.PageObjectModels
{
    class PaymentPage
    {
        private const int WaitSeconds = 20;
        private readonly IWebDriver Driver;
        private const string PageUrl = "https://www.demo.bnz.co.nz/client/payment";
        private const string PageTitle = "Internet Banking";

        public By PaymentFormLocator = By.Id("paymentForm");
        public By AmountLocator = By.XPath("//*[@id=\"field-bnz-web-ui-toolkit-9\"]");
        public By TransferLocator = By.XPath("/html/body/div[8]/div/div/div/div/div[2]/form/div[4]/div/button[1]");

        public By everyDayCurrentBal = By.XPath(
            "/html/body/div[2]/div/div/div[3]/div[2]/div[2]/div/div[2]/div[2]/div/div/div/div/div/div[2]/div[1]/div[2]/span[3]");

        public By billsCurrentBal =
            By.XPath(
                "/html/body/div[2]/div/div/div[3]/div[2]/div[2]/div/div[2]/div[2]/div/div/div/div/div/div[2]/div[3]/div[2]/span[3]");
        public By FromAccountBefTransBal =
            By.XPath("/html/body/div[8]/div/div/div/div/div[1]/div/div[1]/button/div/div/div[2]/p[2]");
        public By ToAccountBefTransBal = 
            By.XPath("/html/body/div[8]/div/div/div/div/div[1]/div/div[2]/button/div/div/div[2]/p[2]");
        public IWebElement ChooseFromAccount => Driver.FindElement(By.ClassName("content-1-1-35"));
        public IWebElement ChooseToAccount => Driver.FindElement(By.XPath("/html/body/div[8]/div/div/div/div/div[1]/div/div[2]/button/div/div/div[2]/p/span"));
        public IWebElement EverydayAccount => Driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/div/div/ul/li[2]/button/div/div/div[2]/p[1]"));
        public IWebElement BillsAccount => Driver.FindElement(By.XPath("/html/body/div[8]/div/div/div[2]/div/div/div[2]/div[2]/ul/li[1]/button/div/div/div[2]/p[1]"));
        public IWebElement AccountsTab => Driver.FindElement(By.Id("react-tabs-2"));
        public void EnterAmount(string amount) => Driver.FindElement(AmountLocator).SendKeys(amount);
        public void ClickPayOrTransfer() => Driver.FindElement(TransferLocator).Click();

        public double FromAccBefBal;

        public double ToAccBefBal;
        public double FromAccBalAft;
        public double ToAccBalAft;

        public PaymentPage(IWebDriver driver)
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

        public void ChooseFromEverydayAccount()
        {
            WaitForElementToBeVisible(By.ClassName("content-1-1-35"), WaitSeconds);
            ChooseFromAccount.Click();

            WaitForElementToBeVisible(By.ClassName("chooserContainer-1-1-22"), WaitSeconds);
            EverydayAccount.Click();
        }

        public void ChooseToBillsAccount()
        {
            WaitForElementToBeVisible(By.ClassName("title-1-1-30"), WaitSeconds);
            ChooseToAccount.Click();

            DemoHelper.Pause();

            WaitForElementToBeVisible(By.Id("react-tabs-2"), WaitSeconds);
            AccountsTab.Click();


            WaitForElementToBeVisible(By.ClassName("content-1-1-64"), WaitSeconds);
            BillsAccount.Click();
        }

        public void GetFromAccBalanceBefTransfer()
        {
            string fromAccBefTransBal = Driver.FindElement(FromAccountBefTransBal).Text;
            var resValue = Regex.Match(fromAccBefTransBal, @"\d+(.\d+)?").Value;
            FromAccBefBal = double.Parse(resValue);

        }

        public void GetToAccBalanceBefTransfer()
        {
            string toAccBefTransBal = Driver.FindElement(ToAccountBefTransBal).Text;
            var resValue2 = Regex.Match(toAccBefTransBal, @"\d+(.\d+)?").Value;
            ToAccBefBal = double.Parse(resValue2);
        }
        public double GetFromAccBalanceAfterTransfer()
        {
            FromAccBalAft = Convert.ToDouble(Driver.FindElement(everyDayCurrentBal).Text);
            return FromAccBalAft;
        }

        public double GetToAccBalanceAfterTransfer()
        {
            var toAccBalAft = Convert.ToDouble(Driver.FindElement(billsCurrentBal).Text);
            return toAccBalAft;
        }


        public bool VerifyCorrectFromAccountBalance(double transferAmount)
        {
            var expectedFromBal = FromAccBefBal - transferAmount;
            FromAccBalAft = GetFromAccBalanceAfterTransfer();

            if (FromAccBalAft == expectedFromBal)
                return true;
            else
                return false;
        }

        public bool VerifyCorrectToAccountBalance(double transferAmount)
        {
            var expectedToBal = ToAccBefBal + transferAmount;
            ToAccBalAft = GetToAccBalanceAfterTransfer();

            if (ToAccBalAft == expectedToBal)
                return true;
            else
                return false;
        }

        public void WaitForElementToBeVisible(By locator, int waitForSeconds)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(waitForSeconds));
            wait.Until(ExpectedConditions.ElementIsVisible(locator));
        }

        public void WaitForElementToBeClickable(By locator, int waitForSeconds)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(waitForSeconds));
            wait.Until(ExpectedConditions.ElementToBeClickable(locator));
        }
    }
}
