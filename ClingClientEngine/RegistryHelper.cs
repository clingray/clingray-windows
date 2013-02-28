using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;

namespace ClingClientEngine {
    public class RegistryHelper {
        public const string LOCAL_REPO_CONTAINER_PATH_REG_NAME = @"LocalRepoContainerPath";
        public const string REPOSITORY_INFO_TIMESTAMP = @"RepositoryInfoTimestamp";

        public static RegistryKey ROOT_REGISTRY_KEY = Registry.LocalMachine;
        public const string CLINGRAY_KEY_NAME = "Clingray";
        /// <summary>
        /// Base registry key name. Registry path will be result of Path.Combine(ROOT_REGISTRY_KEY.Name, "SOFTWARE", baseRegistryKeyName, CLINGRAY_KEY_NAME)
        /// </summary>
        public string baseRegistryKeyName { get; set; }
        

        #region Singleton
        static RegistryHelper _instance = new RegistryHelper();

        RegistryHelper() {
        }

        public static RegistryHelper instance {
            get {
                return _instance;
            }
        }
        #endregion

        public string getSubRegistryPath() {
            if (String.IsNullOrEmpty(baseRegistryKeyName)) {
                throw new InvalidOperationException("baseRegistryKeyName must be SET!");
            }

            return Path.Combine("SOFTWARE", baseRegistryKeyName, CLINGRAY_KEY_NAME);
        }

        #region Key handling
        /// <summary>
        /// Open sub key.
        /// </summary>
        /// <param name="keyName">key name</param>
        /// <param name="writable">writable access mode flag</param>
        /// <returns>RegistryKey. Consumer must call Close() of return value later.</returns>
        public RegistryKey openKey(string keyName = "", bool writable = false) {
            string targetSubPath = Path.Combine(getSubRegistryPath(), keyName);

            return ROOT_REGISTRY_KEY.OpenSubKey(targetSubPath, writable);
        }

        /// <summary>
        /// Create sub key.
        /// </summary>
        /// <param name="keyName">key name</param>
        /// <returns>RegistryKey. Consumer must call Close() of return value later.</returns>
        public RegistryKey createKey(string keyName) {
            RegistryKey key = openKey(keyName);
            if (key == null) {
                string targetSubPath = Path.Combine(getSubRegistryPath(), keyName);
                key = ROOT_REGISTRY_KEY.CreateSubKey(targetSubPath);
            }

            return key;
        }

        /// <summary>
        /// Delete sub key.
        /// </summary>
        /// <param name="keyName">key name</param>
        public void deleteKey(string keyName) {
            RegistryKey parentKey = ROOT_REGISTRY_KEY.OpenSubKey(getSubRegistryPath(), true);

            if(parentKey != null) {
                parentKey.DeleteSubKey(keyName, false);
                parentKey.Close();
            }
        }
        #endregion

        #region Value handling
        public object getValue(string name, string keyName = "") {
            RegistryKey key = openKey(keyName);
            if (key != null) {
                object value = key.GetValue(name);
                key.Close();
                return value;
            }

            return null;
        }

        public long getLongValue(string name, string keyName = "") {
            return long.Parse((string)getValue(name, keyName));
        }

        public void setValue(string name, object value, string keyName = "") {
            RegistryKey key = openKey(keyName, true);
            if (key == null) {
                key = createKey(keyName);
            }

            key.SetValue(name, value);
            key.Close();
        }

        public void deleteValue(string name, string keyName = "") {
            RegistryKey key = openKey(keyName, true);
            if (key != null) {
                key.DeleteValue(name);
                key.Close();
            }
        }
        #endregion
    }
}
