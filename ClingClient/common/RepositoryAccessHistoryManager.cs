using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClingClientEngine;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace ClingClient.common
{
    public class RepositoryAccessHistoryManager
    {
        public const int MAX_HISTORY_COUNT = 3;
        private string ACCESS_HISTORY_DIR;
        private string ACCESS_HISTORY_FILE_PATH;

        public SortedSet<AccessHistory> historySet { get; private set; }

        #region Singleton
        static RepositoryAccessHistoryManager _instance = new RepositoryAccessHistoryManager();

        private RepositoryAccessHistoryManager()
        {
            ACCESS_HISTORY_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Application.CompanyName, Application.ProductName);
            ACCESS_HISTORY_FILE_PATH = Path.Combine(ACCESS_HISTORY_DIR, "RepositoryAccessHistory.xml");

            if (!Directory.Exists(ACCESS_HISTORY_DIR))
            {
                Directory.CreateDirectory(ACCESS_HISTORY_DIR);
            }

            historySet = new SortedSet<AccessHistory>(new AccessHistoryComparer());
            loadHistory();
        }

        public static RepositoryAccessHistoryManager instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        public sealed class AccessHistory
        {
            public string repositoryUUID { get; set; }
            public string accessTime { get; set; }

            public AccessHistory()
            {
            }

            public AccessHistory(string repositoryUUID, DateTime accessTime)
            {
                this.repositoryUUID = repositoryUUID;
                this.accessTime = accessTime.ToUniversalTime().ToString("o");
            }

            public override bool Equals(object obj)
            {
                return ((AccessHistory)obj).repositoryUUID.Equals(this.repositoryUUID);
            }

            public override int GetHashCode()
            {
                return this.repositoryUUID.GetHashCode();
            }
        }

        private sealed class AccessHistoryComparer : IComparer<AccessHistory>
        {
            public int Compare(AccessHistory x, AccessHistory y)
            {
                // To make repositories unique.
                if (x.repositoryUUID.Equals(y.repositoryUUID))
                {
                    return 0;
                }

                return -x.accessTime.CompareTo(y.accessTime); // descending order
            }
        }

        public void addHistory(string repositoryUUID, DateTime accessTime, bool shouldFlush = true)
        {
            Debug.Assert(repositoryUUID != null, "Invalid parameter");
            Debug.Assert(accessTime != null, "Invalid parameter");

            if (historySet.Count == MAX_HISTORY_COUNT)
            {
                historySet.Remove(historySet.Last()); // Remove the oldest element.
            }

            AccessHistory history = new AccessHistory(repositoryUUID, accessTime);
            historySet.Remove(history);
            historySet.Add(history);

            if (shouldFlush)
            {
                saveHistory();
            }
        }

        private void loadHistory()
        {
            historySet.Clear();
            if (File.Exists(ACCESS_HISTORY_FILE_PATH))
            {
                FileStream stream = new FileStream(ACCESS_HISTORY_FILE_PATH, FileMode.Open, FileAccess.Read);
                XmlSerializer serializer = new XmlSerializer(typeof(List<AccessHistory>));
                List<AccessHistory> histories = (List<AccessHistory>)serializer.Deserialize(stream);
                stream.Close();
                foreach (AccessHistory history in histories)
                {
                    addHistory(history.repositoryUUID, DateTime.Parse(history.accessTime).ToUniversalTime(), false);
                }
            }
        }

        private void saveHistory()
        {
            List<AccessHistory> histories = historySet.ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(List<AccessHistory>));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("","");

            FileStream stream = new FileStream(ACCESS_HISTORY_FILE_PATH, FileMode.Create, FileAccess.Write);
            serializer.Serialize(stream, histories, ns);
            stream.Close();
        }
    }
}
