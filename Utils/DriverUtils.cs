using OpenQA.Selenium;
using System;
using System.IO;
using System.Text.RegularExpressions;
using OpenQA.Selenium.Support.UI;

namespace Framework.Utils
{
    public class DriverUtils
    {
        public static object TimeUnit { get; private set; }

        public static string GetScreenshot(IWebDriver driver)
        {
            return GetScreenshot(driver, "", "", true);
        }
        public static string GetScreenshot(IWebDriver driver, Exception ex)
        {
            return GetScreenshot(driver, "", ex.Message, true);
        }

        public static string GetScreenshot(IWebDriver driver, string name)
        {
            return GetScreenshot(driver, "", name, false);
        }

        public static string GetScreenshot(IWebDriver driver, string subDirectory, string name)
        {
            return GetScreenshot(driver, subDirectory, name, false);
        }

        public static string GetScreenshot(IWebDriver driver, string subDirectory, string name, bool makeUnique)
        {
            string screenshotDirectory = "Screenshots" + "\\" + subDirectory;
            name = name.Split('\n')[0];
            name = Regex.Replace(name, @"[^0-9a-zA-Z\._.]", "_").Trim(' ').Split('\n')[0];
            DateTime dt = DateTime.Parse(DateTime.Now.ToString());

            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var environmentDir = new DirectoryInfo(baseDir);
            environmentDir.CreateSubdirectory(screenshotDirectory);

            string dirPath = environmentDir.ToString() + screenshotDirectory + "\\";
            try
            {
                Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
                string filename;
                if (makeUnique) filename = dirPath + name + DateTime.Now.ToString("yyyyMMdd") + dt.ToString("HHmmss");
                else filename = dirPath + name + ".png";
                ss.SaveAsFile(filename, ScreenshotImageFormat.Png);
                Console.WriteLine("screenshot save - " + filename);
                return filename;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to take screenshot");
                throw ex;
            }
        }

        /// <summary>
        /// Wait for page to load
        /// </summary>
        /// <param name="driver">web driver</param>
        public static void WaitForLoad(IWebDriver driver, int timeoutSec = 60)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            WebDriverWait wait = new WebDriverWait(driver, new TimeSpan(0, 0, timeoutSec));
            wait.Until(wd => js.ExecuteScript("return document.readyState").ToString() == "complete");
        }

    }
}
