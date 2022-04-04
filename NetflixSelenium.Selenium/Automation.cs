using NetflixSelenium.Selenium.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetflixSelenium.Selenium
{
    public class Automation
    {
        private int errorCounter = 0;
        private IWebDriver driver;
        private string _url { get; set; } = "10.180.53.47:1234";
        public Automation()
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            firefoxOptions.BrowserExecutableLocation = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            driver = new RemoteWebDriver(new Uri($"http://{_url}/wd/hub"), firefoxOptions);
        }
        public Automation(string url)
        {
            FirefoxOptions firefoxOptions = new FirefoxOptions();
            firefoxOptions.BrowserExecutableLocation = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            driver = new RemoteWebDriver(new Uri($"http://{url}/wd/hub"), firefoxOptions);
        }
        /// <summary>
        /// Creates a New Netflix Account
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ResponseModel CreateAccount(AccountInformation account)
        {
            bool result = false;
            Goto("https://www.netflix.com/signup");
            result = GotoPlanForm();
            if (result)
            {
                result = GotoRegistrationForm();
                if (result)
                {
                    errorCounter = 0;
                    result = StartFillingForm();
                    if (result)
                    {
                        errorCounter = 0;
                        result = FillRegistrationForm(account.EmailAddress, account.Password);
                        if (result)
                        {
                            errorCounter = 0;
                            result = GotoCreditOption();
                            if (result)
                            {
                                errorCounter = 0;
                                result = FillCardInformation(account.FirstName, account.LastName, account.CardNumber, $"{account.Month}/{account.Year}", account.CVV);
                                if (result)
                                {
                                    errorCounter = 0;
                                    result = FillOTP("");
                                    driver.Quit();
                                    return new ResponseModel() { ErrorMessage = "Success" };
                                }
                                else
                                {
                                    driver.Quit();
                                    return new ResponseModel() { ErrorMessage = "Success" };
                                }
                            }
                            else
                            {
                                driver.Quit();
                                return new ResponseModel() { ErrorMessage = "Account Already Exist" };
                            }
                        }
                        else
                        {
                            driver.Quit();
                            return new ResponseModel() { ErrorMessage = "Account Already Exist" };
                        }
                    }
                }
            }
            driver.Quit();
            return new ResponseModel() { ErrorMessage = "Couldn't create account" };
        }

        public async Task<ResponseModel> CreateAccountAsync(AccountInformation account)
        {
            ResponseModel responseModel = new ResponseModel();
            await Task.Run(() =>
            {
                bool result = false;
                Goto("https://www.netflix.com/signup");
                result = GotoPlanForm();
                if (result)
                {
                    result = GotoRegistrationForm();
                    if (result)
                    {
                        errorCounter = 0;
                        result = StartFillingForm();
                        if (result)
                        {
                            errorCounter = 0;
                            result = FillRegistrationForm(account.EmailAddress, account.Password);
                            if (result)
                            {
                                errorCounter = 0;
                                result = GotoCreditOption();
                                if (result)
                                {
                                    errorCounter = 0;
                                    result = FillCardInformation(account.FirstName, account.LastName, account.CardNumber, $"{account.Month}/{account.Year}", account.CVV);
                                    if (result)
                                    {
                                        errorCounter = 0;
                                        result = FillOTP("");
                                        driver.Quit();
                                        responseModel = new ResponseModel() { ErrorMessage = "Success" };
                                    }
                                    else
                                    {
                                        driver.Quit();
                                        responseModel = new ResponseModel() { ErrorMessage = "Success" };
                                    }
                                }
                                else
                                {
                                    driver.Quit();
                                    responseModel = new ResponseModel() { ErrorMessage = "Account Already Exist" };
                                }
                            }
                            else
                            {
                                driver.Quit();
                                responseModel = new ResponseModel() { ErrorMessage = "Account Already Exist" };
                            }
                        }
                    }
                }
                responseModel = new ResponseModel() { ErrorMessage = "Couldn't create account" };
            });
            return responseModel;
        }

        private bool GotoPlanForm()
        {
            if (IsSignupForm)
            {
                clickButton("/html/body/div[1]/div/div/div[2]/div/div[2]/button");
                Thread.Sleep(500);
                return true;
            }
            else
            {
                errorCounter += 1;
                if (errorCounter <= 200)
                    return GotoPlanForm();
            }
            return false;
        }
        private bool GotoRegistrationForm()
        {
            if (IsPlanForm)
            {
                clickButton("/html/body/div[1]/div/div/div[2]/div/div[2]/button");
                Thread.Sleep(500);
                return true;
            }
            else
            {
                errorCounter += 1;
                if (errorCounter <= 200)
                    return GotoRegistrationForm();
            }
            return false;
        }
        private bool StartFillingForm()
        {
            if (IsRegistrationFormStart)
            {
                clickNext();
                Thread.Sleep(500);
                return true;
            }
            else
            {
                errorCounter += 1;
                if (errorCounter <= 200)
                    return StartFillingForm();
            }
            return false;
        }
        private bool FillRegistrationForm(string email, string password)
        {
            if (IsRegistrationForm)
            {
                InputText("email", email);
                InputText("password", password);
                clickButton("/html/body/div[1]/div/div/div[2]/div/form/div/div[2]/button");
                Thread.Sleep(500);
                return true;
            }
            else
            {
                errorCounter += 1;
                if (errorCounter <= 2000)
                    return FillRegistrationForm(email, password);
            }
            return false;
        }
        private bool GotoCreditOption()
        {
            if (IsPaymentForm)
            {
                clickButton("/html/body/div[1]/div/div/div[2]/div/div/div[3]/div[2]/div[1]/a");
                Thread.Sleep(500);
                return true;
            }
            else
            {
                errorCounter += 1;
                if (errorCounter <= 50)
                    return GotoCreditOption();
            }
            return false;
        }
        private bool FillCardInformation(string firstName, string lastName, string ccNum, string ccMonthYear, string cvv)
        {
            if (IsCreditOptionForm)
            {
                InputText("firstName", firstName);
                InputText("lastName", lastName);
                InputText("creditCardNumber", ccNum);
                InputText("creditExpirationMonth", ccMonthYear);
                InputText("creditCardSecurityCode", cvv);
                InputText("creditZipcode", "10080");//zipcode
                clickCheckBox("cb_hasAcceptedTermsOfUse");
                clickButton("/html/body/div[1]/div/div/div[2]/div/form/div[2]/button");
                Thread.Sleep(500);
                return true;
            }
            else
            {
                errorCounter += 1;
                if (errorCounter <= 200)
                    return FillCardInformation(firstName, lastName, ccNum, ccMonthYear, cvv);
            }
            return false;
        }
        private bool FillOTP(string phoneNumber)
        {
            if (IsOTPForm)
            {
                InputText("phoneNumber", phoneNumber);
                clickButton("/html/body/div[1]/div/div/div[2]/div/div[2]/button");
                return true;
            }
            return false;
        }
        private void Goto(string url)
        {
            driver.Navigate().GoToUrl(url);
        }
        private void InputText(string elementName, string text)
        {
            try
            {
                IWebElement textBox = driver.FindElement(By.Name(elementName));
                textBox.SendKeys(text);
            }
            catch (Exception)
            {
            }

        }
        private void clickCheckBox(string name)
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                var last_height = js.ExecuteScript("return document.body.scrollHeight");
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Actions actions = new Actions(driver);
                IWebElement radioBtn = driver.FindElement(By.XPath("//*[@id=\"cb_hasAcceptedTermsOfUse\"]"));
                actions.MoveToElement(radioBtn).Click().Build().Perform();
            }
            catch (Exception)
            {
                Thread.Sleep(500);
                clickCheckBox(name);
            }
        }
        private void clickNext()
        {
            try
            {
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                var last_height = js.ExecuteScript("return document.body.scrollHeight");
                js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
                Actions actions = new Actions(driver);
                IWebElement radioBtn = driver.FindElement(By.XPath("/html/body/div[1]/div/div/div[2]/div/div[2]/button"));
                actions.MoveToElement(radioBtn).Click().Build().Perform();
            }
            catch (Exception)
            {
                Thread.Sleep(500);
                clickNext();
            }
        }
        private void clickButton(string query)
        {

            try
            {
                IWebElement getStartedBtn = driver.FindElement(By.XPath(query));
                getStartedBtn.Click();
            }
            catch (StaleElementReferenceException)
            {
                Thread.Sleep(500);
                clickButton(query);
            }
            catch (ElementNotInteractableException)
            {
                Thread.Sleep(500);
                clickButton(query);
            }
        }
        private bool IsRegistrationFormStart
        {
            get
            {
                string currectURL = driver.Url;
                if (currectURL.Contains("https://www.netflix.com/signup/registration"))
                    return true;
                return false;
            }
        }
        private bool IsRegistrationForm
        {
            get
            {
                string currectURL = driver.Url;
                if (currectURL.Contains("https://www.netflix.com/signup/regform"))
                    return true;
                return false;
            }
        }
        private bool IsPaymentForm
        {
            get
            {
                string currectURL = driver.Url;
                if (currectURL.Contains("https://www.netflix.com/signup/payment"))
                    return true;
                return false;
            }
        }
        private bool IsCreditOptionForm
        {
            get
            {
                string currectURL = driver.Url;
                if (currectURL.Contains("https://www.netflix.com/signup/creditoption"))
                    return true;
                return false;
            }
        }
        private bool IsSignupForm
        {
            get
            {
                string currentURL = driver.Url;
                if (currentURL.Contains("https://www.netflix.com/signup"))
                    return true;
                return false;
            }
        }
        private bool IsPlanForm
        {
            get
            {
                string currentURL = driver.Url;
                if (currentURL.Contains("https://www.netflix.com/signup/planform"))
                    return true;
                return false;
            }
        }
        private bool IsOTPForm
        {
            get
            {
                string currentURL = driver.Url;
                if (currentURL.Contains("https://www.netflix.com/signup/otpPhoneEntry"))
                    return true;
                return false;
            }
        }
    }
}
