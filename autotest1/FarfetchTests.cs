using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System.Text.RegularExpressions;

namespace AutomatedTests
{
    public class FarfetchTests
    {
        WebDriver driver;

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.PageLoadStrategy = PageLoadStrategy.Normal;
            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
            driver.Manage().Window.Maximize();
            driver.Navigate().GoToUrl("https://www.farfetch.com/ru");
        }

        [Test]
        public void TestPriceFilter()
        {
            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElement(By.XPath("//a[@data-test='header-gender-tab-248']")));
            driver.FindElement(By.XPath("//a[@data-test='header-gender-tab-248']")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElement(By.XPath("//*[@data-nav='/ru/shopping/men/shoes-2/items.aspx']")));
            driver.FindElement(By.XPath("//*[@data-nav='/ru/shopping/men/shoes-2/items.aspx']")).Click();

            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElement(By.XPath("//button[@aria-label='Close Dialog']")));
            driver.FindElement(By.XPath("//button[@aria-label='Close Dialog']")).Click();
            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElement(By.XPath("//*[@data-testid='priceHeader']")));
            driver.FindElement(By.XPath("//*[@data-testid='priceHeader']")).Click();

            driver.FindElement(By.XPath("//*[@data-testid='price-input-min']")).Clear();
            driver.FindElement(By.XPath("//input[@data-testid='price-input-min']")).SendKeys("10000");
            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElement(By.XPath("//input[@data-testid='price-input-max']")));
            driver.FindElement(By.XPath("(//input[@data-testid='price-input-max'])")).Clear();
            driver.FindElement(By.XPath("(//input[@data-testid='price-input-max'])")).SendKeys("45000");
            driver.FindElement(By.XPath("(//input[@data-testid='price-input-max'])")).SendKeys(Keys.Enter);

            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElements(By.XPath("//*[@data-component='Price']")));
            var webPrices = driver.FindElements(By.XPath("//*[@data-component='Price']"));
            new WebDriverWait(driver, TimeSpan.FromSeconds(40)).Until(x => driver.FindElements(By.XPath("//*[@data-component='PriceOriginal']")));
            if (driver.FindElements(By.XPath("//*[@data-component='PriceOriginal']")).Any()) webPrices = driver.FindElements(By.XPath("//*[@data-component='PriceFinal']"));

            int[] actualPrices = webPrices.Select(webPrice => Int32.Parse(Regex.Replace(webPrice.Text, @"\D+", ""))).ToArray();
            actualPrices.ToList().ForEach(price => Assert.IsTrue(price >= 10000 && price <= 45000));
        }

        [Test]
        public void TestAddToCartButtonTooltipText()
        {
            driver.FindElement(By.XPath("//button[@data-tstid='country_selector_btn_desktop']")).Click();
            Assert.IsTrue(driver.FindElements(By.XPath(".//*[@class='_e9e1a6 css-1e3yvoj-Kicker e1lqpzq10']")).Any(),
                "Tooltip on 'Country Selector' has not appeared");
            Assert.AreEqual(driver.FindElement(By.XPath(".//*[@class='_e9e1a6 css-1e3yvoj-Kicker e1lqpzq10']")).Text.Trim(), "СТРАНА ИЛИ РЕГИОН ДОСТАВКИ",
                "Incorrect tooltip text");
        }

        [Test]
        public void NegativeTestPhoneNumberConfirmationWithEmptyPhoneNumber()
        {
            driver.FindElement(By.XPath("//button[@data-test='go-to-login-desktop']")).Click();
            driver.FindElement(By.XPath("//*[@id='tabs--2--tab--1']")).Click();
            driver.FindElement(By.XPath("//*[@name='name']")).SendKeys("Test");
            driver.FindElement(By.XPath("//*[@id='register-email']")).SendKeys("shjfgshjds534.yhft@mail.ru");
            driver.FindElement(By.XPath("//*[@name='password']")).SendKeys("Autotest");
            Assert.IsTrue(driver.FindElements(By.XPath("//button[@type = 'submit'and not(@disabled)]")).Any(),
                "Button 'Зарегистрироваться' is enabled when input fields is empty");
        }

        [TearDown]
        public void CleanUp()
        {
            driver.Quit();
        }
    }
}