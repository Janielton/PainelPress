using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using PainelPress.Model;

namespace PainelPress.Classes
{
    public class ClassSeleniun
    {
            private static IWebDriver driver;
            private StringBuilder verificationErrors;
            private static string baseURL;
            private bool acceptNextAlert = true;
            private bool hiden = true;

        public ClassSeleniun()
        {
            var option = new ChromeOptions();
            option.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            option.AddArguments("--test-type", "--start-minimized");
            option.AddArguments("--test-type", "--ignore-certificate-errors");
            option.AddArgument("headless");
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Constants.PASTA);
            chromeDriverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(chromeDriverService, option);
            baseURL = "https://www.google.com/";

       }
        public ClassSeleniun(bool hidenBW)
        {
            hiden = hidenBW;
            var option = new ChromeOptions();
            option.BinaryLocation = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            option.AddArguments("--test-type", "--start-minimized");
            option.AddArguments("--test-type", "--ignore-certificate-errors");
            if(hiden) option.AddArgument("headless");
            var chromeDriverService = ChromeDriverService.CreateDefaultService(Constants.PASTA);
            chromeDriverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(chromeDriverService, option);
            baseURL = "https://www.google.com/";

        }
        public static void CleanupClass()
            {
                try
                {
                    //driver.Quit();// quit does not close the window
                    driver.Close();
                    driver.Dispose();
                }
                catch (Exception)
                {
                    // Ignore errors if unable to close the browser
                }
       }


            public void InitializeTest()
            {
                verificationErrors = new StringBuilder();
            }

  
            public void CleanupTest()
            {
                
            }

            public bool PostaFace(string texto)
            {
             try
            {
                var dados = new SocialModel().facebook();
                driver.Navigate().GoToUrl("https://mbasic.facebook.com/");
                driver.FindElement(By.Id("m_login_email")).Click();
                driver.FindElement(By.Id("m_login_email")).Clear();
                // driver.FindElement(By.Id("m_login_email")).SendKeys(dados.usuario);
                driver.FindElement(By.Name("pass")).Click();
                driver.FindElement(By.Name("pass")).Clear();
               // driver.FindElement(By.Name("pass")).SendKeys(dados.senha);
                driver.FindElement(By.Name("login")).Click();
                driver.Navigate().GoToUrl($"https://iphone.facebook.com/{dados.pagina}/posts");
                Thread.Sleep(1000);
                Actions action = new Actions(driver);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.Perform();
                action.SendKeys(Keys.Enter);
                action.Perform();
                Thread.Sleep(2000);
                driver.FindElement(By.Name("status")).Click();
                // driver.FindElement(By.Name("xc_message")).Clear();
                driver.FindElement(By.Name("status")).SendKeys(texto);
                Thread.Sleep(1000);
                driver.FindElement(By.ClassName("_9drg")).Click();
                Thread.Sleep(2000);
                if(hiden) driver.Quit();

                return true;
             }
             catch(Exception ex) {
                Debug.WriteLine(ex.Message);
                driver.Quit();
                return false; }

            }

        public bool PostaTwitter(string texto)
        {
            try
            {
                var dados = new SocialModel().facebook();
                driver.Navigate().GoToUrl("https://mbasic.facebook.com/");
                driver.FindElement(By.Id("m_login_email")).Click();
                driver.FindElement(By.Id("m_login_email")).Clear();
                // driver.FindElement(By.Id("m_login_email")).SendKeys(dados.usuario);
                driver.FindElement(By.Name("pass")).Click();
                driver.FindElement(By.Name("pass")).Clear();
                // driver.FindElement(By.Name("pass")).SendKeys(dados.senha);
                driver.FindElement(By.Name("login")).Click();
                driver.Navigate().GoToUrl($"https://iphone.facebook.com/{dados.pagina}/posts");
                Thread.Sleep(1000);
                Actions action = new Actions(driver);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.SendKeys(Keys.Tab);
                action.Perform();
                action.SendKeys(Keys.Enter);
                action.Perform();
                Thread.Sleep(2000);
                driver.FindElement(By.Name("status")).Click();
                // driver.FindElement(By.Name("xc_message")).Clear();
                driver.FindElement(By.Name("status")).SendKeys(texto);
                Thread.Sleep(1000);
                driver.FindElement(By.ClassName("_9drg")).Click();
                Thread.Sleep(2000);
                if (hiden) driver.Quit();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                driver.Quit();
                return false;
            }

        }

        private bool IsLogadoTwitter()
        {
            driver.Navigate().GoToUrl("https://twitter.com/i/bookmarks");
            Debug.WriteLine(driver.Url);
            return true;
        }

        private bool IsElementPresent(By by)
            {
                try
                {
                    driver.FindElement(by);
                    return true;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
            }

            private bool IsAlertPresent()
            {
                try
                {
                    driver.SwitchTo().Alert();
                    return true;
                }
                catch (NoAlertPresentException)
                {
                    return false;
                }
            }

            private string CloseAlertAndGetItsText()
            {
                try
                {
                    IAlert alert = driver.SwitchTo().Alert();
                    string alertText = alert.Text;
                    if (acceptNextAlert)
                    {
                        alert.Accept();
                    }
                    else
                    {
                        alert.Dismiss();
                    }
                    return alertText;
                }
                finally
                {
                    acceptNextAlert = true;
                }
            }
        }
    }
