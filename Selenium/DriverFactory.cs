using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Remote;
using Framework.Config;
using System;
using TestFramework.Data;

namespace Framework.Selenium
{
    public class DriverFactory
    {
        // driver capabilities - https://github.com/SeleniumHQ/selenium/wiki/DesiredCapabilities

        static String baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        static String hub = "http://10.140.164.82:4444/wd/hub";

        public static bool Local()
        {
            return AppConfig.session.Contains("local");
        }

        public static bool Remote()
        {
            return AppConfig.session.Contains("remote");
        }

        public static bool Sauce()
        {
            return AppConfig.session.Contains("sauce");
        }

        public static IWebDriver GetChromeDriver()
        {
            switch (AppConfig.session)
            {
                case "local":
                    return new ChromeDriver(baseDirectory);
                case "grid":
                    var chrome_caps = new ChromeOptions().ToCapabilities();
                    return new RemoteWebDriver(new Uri(hub), chrome_caps);
                case "sauce":
                    return GetSauceLabsDriver();
            }
            return null;
        }

        //TODO remote web driver instance throws exception 'invalid ceritifcate'
        public static IWebDriver GetFirefoxDriver()
        {
            Environment.SetEnvironmentVariable("webdriver.gecko.driver", baseDirectory);
            switch (AppConfig.session)
            {
                case "local":
                    FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
                    //service.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    FirefoxOptions options = new FirefoxOptions();
                    TimeSpan time = TimeSpan.FromSeconds(10);
                    return new FirefoxDriver(service, options, time);
                case "grid":
                    var firefox_caps = new FirefoxOptions().ToCapabilities();
                    return new RemoteWebDriver(new Uri(hub), firefox_caps);
                case "sauce":
                    GetSauceLabsDriver();
                    break;
            }
            return null;
        }

        public static IWebDriver GetIEDriver()
        {
            Environment.SetEnvironmentVariable("webdriver.ie.driver", baseDirectory);
            var ie_caps = new InternetExplorerOptions()
            {
                EnsureCleanSession = true,
                IgnoreZoomLevel = true,
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                RequireWindowFocus = true
            };
            ie_caps.ToCapabilities();
            switch (AppConfig.session)
            {
                case "local":
                    return new InternetExplorerDriver(baseDirectory, ie_caps);
                case "grid":
                    return new RemoteWebDriver(new Uri(hub), ie_caps);
                case "sauce":
                    GetSauceLabsDriver();
                    break;
            }
            return null;
        }

        public static IWebDriver GetPhantomJSDriver()
        {
            switch (AppConfig.session)
            {
                case "local":
                    return new PhantomJSDriver();
                case "grid":
                    var phantom_caps = new PhantomJSOptions().ToCapabilities();
                    return new RemoteWebDriver(new Uri(hub), phantom_caps);
                case "sauce":
                    GetSauceLabsDriver();
                    break;
            }
            return null;
        }

        public static IWebDriver GetSauceLabsDriver()
        {
            DesiredCapabilities caps = new DesiredCapabilities();
            caps.SetCapability("browserName", AppConfig.browserName);
            caps.SetCapability("platform", AppConfig.platform);
            caps.SetCapability("version", AppConfig.version);
            caps.SetCapability("username", "cgrandoit");
            caps.SetCapability("accessKey", "");
            return new RemoteWebDriver(new Uri("http://ondemand.saucelabs.com:80/wd/hub"), caps, TimeSpan.FromSeconds(600));
        }


        public static IWebDriver GetWebDriver()
        {
            switch (AppConfig.browserName)
            {
                case "Chrome":
                    return GetWebDriver(Browser.CHROME);
                case "Firefox":
                    return GetWebDriver(Browser.FIREFOX);
                case "Internet Explorer":
                    return GetWebDriver(Browser.IE);
                case "phantomjs":
                    return GetWebDriver(Browser.PHANTOM);
            }
            return null;
        }

        public static IWebDriver GetWebDriver(Browser browser)
        {
            switch (browser)
            {
                case Browser.CHROME:
                    return GetChromeDriver();
                case Browser.FIREFOX:
                    return GetFirefoxDriver();
                case Browser.IE:
                    return GetIEDriver();
                case Browser.PHANTOM:
                    return GetPhantomJSDriver();
            }
            return null;
        }
    }
}
