using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;
using System.Diagnostics;
using System.Collections;
using Framework.Utils;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace Framework.Selenium
{
    public class Elements : IWebElement, IWrapsElement, ILocatable, IEnumerable
    {
        private IWebDriver webDriver;
        private IWebElement element;
        public IList<IWebElement> elements;
        private string locatorType;
        private string locatorPath;

        public Elements(IWebDriver webDriver, string text)
        {
            By by = By.XPath("//*[contains(text(),'" + text + "')]");
            FindWebElements(webDriver, by);
        }

        public Elements(IWebDriver webDriver, string tag, string text)
        {
            By by = By.XPath("//"+tag+"[contains(text(),'" + text + "')]");
            FindWebElements(webDriver, by);
        }

        public Elements(IWebDriver webDriver, By by)
        {
            FindWebElements(webDriver, by);
        }

        public Elements(IWebDriver webDriver, By by, int seconds)
        {
            FindWebElements(webDriver, by, seconds);
        }

        public Elements(IWebDriver webDriver, Elements element, By by)
        {
            FindWebElements(webDriver, element, by);
        }

        public Elements(IWebDriver webDriver, Elements element, By by, int waitTime)
        {
            FindWebElements(webDriver, element, by, waitTime);
        }

        public string TagName => element.TagName;

        public string Text => element.Text;

        public bool Enabled => element.Enabled;

        public bool Selected => element.Selected;

        public Point Location => element.Location;

        public Size Size => element.Size;

        public bool Displayed => element.Displayed;

        public IWebElement WrappedElement { get => throw new NotImplementedException(); }
        public Point LocationOnScreenOnceScrolledIntoView { get => throw new NotImplementedException(); }
        public ICoordinates Coordinates { get => throw new NotImplementedException(); }

        public void Clear()
        {
            Debug.WriteLine("Clearing  " + locatorPath + " ...");
            try
            {
                element.Clear();
            }
            catch (Exception ex)
            {
                DriverUtils.GetScreenshot(webDriver, "Clear failed clearing " + locatorType + locatorPath);
                throw ex;
            }
        }

        public void Click()
        {
            Debug.WriteLine("Clicking  " + locatorPath + " ...");
            try
            {
                element.Click();
            }
            catch (Exception ex)
            {
                DriverUtils.GetScreenshot(webDriver, "Click failed clicking " + locatorType + locatorPath);
                throw ex;
            }
        }

        public IWebElement FindElement(By by)
        {
            Debug.WriteLine("Looking for element " + locatorPath + " by " + locatorType + " ...");

            try
            {
                return elements.ElementAt(0).FindElement(by);
            }
            catch (Exception ex)
            {
                DriverUtils.GetScreenshot(webDriver, "FindElement failed to locate " + locatorType + locatorPath);
                throw ex;
            }
        }

        public IList<IWebElement> FindWebElements(IWebDriver webDriver, By by)
        {
            return FindWebElements(webDriver, by, 30);
        }

        public IList<IWebElement> FindWebElements(IWebDriver webDriver, Elements elemental, By by)
        {
            return FindWebElements(webDriver, elemental, by, 30);
        }

        public IList<IWebElement> FindWebElements(IWebDriver webDriver, By by, int seconds)
        {
            return FindWebElements(webDriver, null, by, seconds);
        }
        public IList<IWebElement> FindWebElements(IWebDriver webDriver, Elements elemental, By by, int waitTime)
        {
            this.webDriver = webDriver;
            locatorPath = by.ToString().Split(':')[1].Trim();
            locatorType = by.ToString().Split(':')[0].Split('.')[1].Trim();
            Debug.WriteLine("Looking for elements " + locatorPath + " by " + locatorType + " ...");
            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(waitTime));
            wait.IgnoreExceptionTypes(typeof(Exception));
            if (elemental == null ) wait.Until(ExpectedConditions.ElementExists(by));
            element = wait.Until((d) =>
            {
                if (elemental == null)
                {
                    return webDriver.FindElement(by);
                }
                else
                {
                    return elemental.FindElement(by);
                }
            });
            if (element!=null)
            {
                try
                {
                    if (elemental == null)
                    {
                        elements = webDriver.FindElements(by);
                    }
                    else
                    {
                        elements = elemental.FindElements(by);
                    }
                    Debug.WriteLine("Found " + elements.Count + " elements");
                    element = elements.ElementAt(0);
                    return elements;
                }
                catch (Exception ex)
                {
                    DriverUtils.GetScreenshot(webDriver, "FindWebElements failed to locate " + locatorType + locatorPath);
                    throw ex;
                }
            }
            else return null;
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            Debug.WriteLine("Looking for elements " + locatorPath + " within " + elements.ElementAt(0).ToString() + " by " + locatorType + " ...");
            try
            {
                var elementsCollection = elements.ElementAt(0).FindElements(by);
                Debug.WriteLine("Found " + elementsCollection.Count + " elements");
                return elementsCollection;
            }
            catch (Exception ex)
            {
                DriverUtils.GetScreenshot(webDriver, "FindElements failed to locate " + locatorType + locatorPath);
                throw ex;
            }
        }

        public string GetAttribute(string attributeName)
        {
            return element.GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return element.GetCssValue(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            return element.GetProperty(propertyName);
        }

        public void SendKeys(string text)
        {
            Debug.WriteLine("Sending " + text + " to " + locatorPath + " ...");
            try
            {
                element.SendKeys(text);
            }
            catch (Exception ex)
            {
                DriverUtils.GetScreenshot(webDriver, "SendKeys failed typing into " + locatorType + locatorPath);
                throw ex;
            }
        }

        public void Submit()
        {
            element.Submit();
        }

        public IEnumerator GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        public bool ActiveClass()
        {
            return element.GetAttribute("class").Contains("active");
        }
    }
}
