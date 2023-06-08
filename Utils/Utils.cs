using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Framework.Utils
{
    public class Utils
    {
        public static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;

            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());

            if (firstBitmap.Equals(secondBitmap))
                return true;
            else
                return false;
        }

        public static void GoToTab(IWebDriver webDriver, int tab)
        {
            int count = 0;
            string currentUrl = webDriver.Url;
            var browserTabs = webDriver.WindowHandles;
            if (tab > browserTabs.Count) Assert.Fail("cannot switch to tab #:" + tab + ", there are only " + browserTabs.Count + " tab(s) open");
            else webDriver.SwitchTo().Window(browserTabs[tab]);
            while (webDriver.Url.Contains(currentUrl))
            {
                Thread.Sleep(1000);
                count++;
                if (count == 30)
                {
                    Assert.Fail(currentUrl + " still appearing in url bar");
                }
            }
        }
    }
}
