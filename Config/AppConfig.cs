using System.Configuration;

namespace Framework.Config
{
    public class AppConfig
    {
        public static readonly string session = ConfigurationManager.AppSettings["session"];
        public static readonly string browserName = ConfigurationManager.AppSettings["browserName"];
        public static readonly string platform = ConfigurationManager.AppSettings["platform"];
        public static readonly string version = ConfigurationManager.AppSettings["version"];
        public static readonly string env = ConfigurationManager.AppSettings["env"];
        public static readonly string baseUrl = ConfigurationManager.AppSettings["baseUrl"];
        public static readonly string build = ConfigurationManager.AppSettings["build"];
    }
}
