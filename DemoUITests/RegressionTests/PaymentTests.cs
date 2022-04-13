using System;
using DemoUITests.PageObjectModels;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;

namespace DemoUITests
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    [Category("SmokeTests")]
    public class PaymentTests <Multi> where Multi : IWebDriver, new()
    {
        private const int WaitForSeconds = 10;

        [Test]
        [Description("TC5 - Go To Payments and transfer funds")]
        [Parallelizable]
        public void TestNavigateToPayments()
        {
            const string transferAmount = "500";
            using (IWebDriver driver = new Multi())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                PaymentPage paymentPage = homePage.GoToPaymentsPage();
                paymentPage.WaitForElementToBeVisible(paymentPage.PaymentFormLocator, WaitForSeconds);

                //Choose From and Select Everyday Account
                paymentPage.ChooseFromEverydayAccount();

                //Choose To and Select Bills Account
                paymentPage.ChooseToBillsAccount();

                paymentPage.GetFromAccBalanceBefTransfer();
                paymentPage.GetToAccBalanceBefTransfer();

                //Enter 500
                paymentPage.WaitForElementToBeVisible(paymentPage.AmountLocator, WaitForSeconds);
                paymentPage.EnterAmount(transferAmount);

                //Click Transfer
                paymentPage.WaitForElementToBeClickable(paymentPage.TransferLocator, 5);
                paymentPage.ClickPayOrTransfer();

                //Transfer successful message is displayed
                //IAlert alert = driver.SwitchTo().Alert();
                //Assert.Equals("Transfer successful", alert.Text);

                //Verify the current balance of Everyday account and Bills account are correct
                bool isFromAccountCurrentBalanceCorrect = paymentPage.VerifyCorrectFromAccountBalance(Convert.ToDouble(transferAmount));
                bool isToAccountCurrentBalanceCorrect = paymentPage.VerifyCorrectToAccountBalance(Convert.ToDouble(transferAmount));
                Assert.IsTrue(isFromAccountCurrentBalanceCorrect, $"Unexpected From Account balance. Before: ${paymentPage.FromAccBefBal} After: ${paymentPage.FromAccBalAft}");
                Assert.IsTrue(isToAccountCurrentBalanceCorrect, $"Unexpected To Account balance. Before: ${paymentPage.ToAccBefBal} After: $${paymentPage.ToAccBalAft}");
            }
        }

    }
}
