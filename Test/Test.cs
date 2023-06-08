using Framework.Selenium;
using Framework.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Collections.Generic;

namespace Framework.Test
{
    public class Test
    {
        public IWebDriver webDriver;

        [SetUp]
        public void Setup()
        {
            webDriver = DriverFactory.GetWebDriver();
        }

        [Test]
        public void NoDifferenceBetweenImageTwoImages()
        {
            webDriver.Url = "10.151.10.187" + "APQAUnitTest/" + "APQAUnitTest_02" + ".html";
            IWebElement ol = webDriver.FindElement(By.TagName("ol"));
            Elements element = new Elements(webDriver, By.TagName("ol"));
            IList<IWebElement> elements = element.FindElements(By.TagName("li"));
            //IList<Elements> elementsX = element.FindElements(By.TagName("li"));
            IList<IWebElement> listItems = ol.FindElements(By.TagName("li"));
        }


        [TearDown]
        public void Teardown()
        {
            webDriver.Close();
        }
    }
}
