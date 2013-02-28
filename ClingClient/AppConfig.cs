using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using ClingClient.Properties;

namespace ClingClient
{
    public class AppConfig
    {
        public const string Development = "development";
        public const string Inhouse = "inhouse";
        public const string Production = "production";

        private static string distributionType = ConfigurationManager.AppSettings["distributionType"];

        public static bool isDevelopment()
        {
            return distributionType.Equals(Development);
        }

        public static bool isInhouse()
        {
            return distributionType.Equals(Inhouse);
        }

        public static bool isProduction()
        {
            return distributionType.Equals(Production);
        }

        public static bool isInternal()
        {
            return isDevelopment() || isInhouse();
        }

        public static bool isExternnal()
        {
            return !isInternal();
        }
    }
}
