using System.Collections.Generic;
using DemoUITests.PageObjectModels;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace DemoUITests
{
    [TestFixture(typeof(ChromeDriver))]
    [TestFixture(typeof(FirefoxDriver))]
    [TestFixture(typeof(InternetExplorerDriver))]
    [Category("RegressionTests")]
    [Parallelizable]
    public class PayeeTests <Multi> where Multi :IWebDriver, new ()
    {
        private const int WaitForSeconds = 10;
        private const string PayeeName = "John Smith";
        [Test]
        [Description("TC1 - Verify user can navigate to Payees page using navigation menu")]
        public void TestPayeesPageLoad()
        {
            using (IWebDriver driver = new Multi())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                PayeePage payeePage = homePage.GoToPayeesPage();
                payeePage.WaitForElementToBeVisible(payeePage.PayeePageHeader, WaitForSeconds);

                //Verify user is in the Payees page
                payeePage.EnsurePageLoaded();
            }
        }

        [Test]
        [Description("TC2 - Verify user can navigate to Payees page using navigation menu")]
        [Parallelizable]
        public void TestAddNewPayee()
        {
            const string apmBank = "12";
            const string apmBranch = "1234";
            const string apmAccount = "1234567";
            const string apmSuffix = "001";

            using (IWebDriver driver = new Multi())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                PayeePage payeePage = homePage.GoToPayeesPage();
                payeePage.WaitForElementToBeVisible(payeePage.PayeePageHeader, WaitForSeconds);

                //Click Add New Payee button
                payeePage.ClickAddPayeeButton();
                payeePage.WaitForElementToBeVisible(payeePage.ImagePickerLocator, WaitForSeconds);

                //Key in Payee Name and Account Number
                payeePage.EnterPayeeName(PayeeName + "\n");
                payeePage.EnterApmBank(apmBank);
                payeePage.EnterApmBranch(apmBranch);
                payeePage.EnterApmAccount(apmAccount);
                payeePage.EnterApmSuffix(apmSuffix);

                //Submit Form
                payeePage.SubmitNewPayee();

                //Verify confirmation message is displayed
                payeePage.WaitForElementToBeVisible(payeePage.PayeeSuccessMsgLocator, WaitForSeconds);
                //Assert.AreEqual("Payee added", payeePage.PayeeSuccessMsg, "Unexpected confirmation message");

                //Confirm newly added payee was added to the list of payees
                List<string> payeeListNameText = payeePage.GetPayeeListNames();
                List<string> payeeListAccountText = payeePage.GetPayeeListAccounts();

                Assert.That(payeeListNameText.Contains(PayeeName), "Newly added Payee name not found");
                Assert.That(payeeListAccountText.Contains($"{apmBank}-{apmBranch}-{apmAccount}-{apmSuffix}"), "Newly added Payee account number not found");
            }
        }

        [Test]
        [Description("TC3 - Verify that payee name is a required field")]
        [Parallelizable]
        public void TestAddPayeeMissingName()
        {
            using (IWebDriver driver = new Multi())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                PayeePage payeePage = homePage.GoToPayeesPage();
                payeePage.WaitForElementToBeVisible(payeePage.PayeePageHeader, WaitForSeconds);

                //Click Add New Payee button
                payeePage.ClickAddPayeeButton();
                payeePage.WaitForElementToBeVisible(payeePage.ImagePickerLocator, WaitForSeconds);
                
                //Submit Form
                payeePage.SubmitNewPayee();

                //Verify required field validation message is displayed
                payeePage.WaitForElementToBeVisible(payeePage.PayeeErrorTooltipLocator, WaitForSeconds);
                Assert.AreEqual("Payee Name is a required field. Please complete to continue.", payeePage.GetRequiredFieldErrorMsg, "Missing Payee Name required field validation");

                //Populate mandatory field
                payeePage.EnterPayeeName(PayeeName + "\n");

                //Validate validation errors are gone
                payeePage.WaitForElementToDisappear(payeePage.PayeeErrorTooltipLocator, WaitForSeconds);
                Assert.IsFalse(payeePage.RequiredFieldErrorMsg.Displayed);

            }
        }

        [Test]
        [Description("TC4 - Verify that payees can be sorted by name")]
        [Parallelizable]
        public void TestPayeeListSortByName()
        {
            using (IWebDriver driver = new Multi())
            {
                var homePage = new HomePage(driver);
                homePage.NavigateTo();

                PayeePage payeePage = homePage.GoToPayeesPage();
                payeePage.WaitForElementToBeVisible(payeePage.PayeePageHeader, WaitForSeconds);

                //Verify list is sorted by ascending order by default
                payeePage.CheckListInAscOrderByDefault();

                //Click Name Column header
                payeePage.ClickSortByName();

                //Verify list is sorted in descending order
                payeePage.CheckListInDescOrder();
            }
        }
    }
}
