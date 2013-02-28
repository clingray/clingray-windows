using System;
using System.Net;
using System.Runtime.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using ClingClient.utilities;
using System.Net.NetworkInformation;
using System.Threading;

namespace ClingUpdater {
    public class UpdateChecker {
        public const string DEFAULT_UPDATE_HOST = "update.clingray.com";
        public enum DistributionType {
            DEVELOPMENT,
            PRODUCTION
        };

        public string packageName { get; set; }
        public string updateHost { get; set; }
        public DistributionType distributionType { get; set; }

        UpdateInfo _lastUpdateInfo;
        public UpdateInfo lastUpdateInfo {
            get {
                return _lastUpdateInfo;
            }
        }

        public delegate void UpdateCheckCompletionDelegate(UpdateInfo updateInfo);

        const string QUERY_UPDATE_URL_FORMAT = @"http://{0}/queryUpdate?packageType=clingray&distributionType={1}&platform=windows&currentVersion={2}";

        UpdateCheckCompletionDelegate updateQueryCompletionDelegate;

        public UpdateChecker(string packageName,
                            string updateHost = DEFAULT_UPDATE_HOST, 
                            DistributionType distributionType = DistributionType.PRODUCTION) {
            this.packageName = packageName;
            this.updateHost = updateHost;
            this.distributionType = distributionType;
        }

        private string getQueryUpdateUrl() {
            string distributionTypeStr = distributionType == DistributionType.DEVELOPMENT ? "development" : "production";
            string applicationVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            string url = String.Format(QUERY_UPDATE_URL_FORMAT, updateHost, distributionTypeStr, applicationVersion);

            return url;
        }

        public UpdateInfo checkUpdate() {
            UpdateInfo updateInfo = null;
            try {
                ClingWebClient client = new ClingWebClient();
                client.timeout = 10000;
                byte[] xmlBuffer = client.DownloadData(new Uri(getQueryUpdateUrl()));
                updateInfo = deserializeXml(xmlBuffer);

            } catch (WebException) {
                Console.WriteLine("WebException during update check.");
            }

            return updateInfo;
        }

        public void checkUpdate(UpdateCheckCompletionDelegate updateCheckCompletionDelegate) {
            if (SystemUtils.isNetworkAvailable())
            {
                try
                {
                    ClingWebClient client = new ClingWebClient();
                    client.timeout = 5000;
                    client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(updateCheckCompleted);
                    client.DownloadDataAsync(new Uri(getQueryUpdateUrl()));

                    this.updateQueryCompletionDelegate = updateCheckCompletionDelegate;
                }
                catch (WebException)
                {
                    Console.WriteLine("WebException during update check.");
                    updateCheckCompletionDelegate(null);
                }
            }
            else
            {
                updateCheckCompletionDelegate(null);
            }
        }

        UpdateInfo deserializeXml(byte[] xmlBuffer) {
            UpdateInfo updateInfo = null;
            MemoryStream stream = null;
            try {
                stream = new MemoryStream(xmlBuffer);
                XmlSerializer serializer = new XmlSerializer(typeof(UpdateInfo));
                updateInfo = (UpdateInfo)serializer.Deserialize(stream);

                if (updateInfo != null) {
                    _lastUpdateInfo = updateInfo;
                }
            } catch (Exception) {
                Console.WriteLine("IOException during update query.");
            } finally {
                if (stream != null) {
                    stream.Close();
                }    
            }

            return updateInfo;
        }

        void updateCheckCompleted(object sender, DownloadDataCompletedEventArgs args) {
            if (args.Error == null) {
                UpdateInfo updateInfo = null;
                byte[] updateInfoXml = args.Result;

                if (updateInfoXml != null) {
                    updateInfo = deserializeXml(updateInfoXml);
                }

                updateQueryCompletionDelegate(updateInfo); 
            }
        }       
    }
}
