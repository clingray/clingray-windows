using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ClingClientEngine {
    class RepoUtils {
        public static string getLocalRepoAbsolutePath(string repositoryUUID) {
            string basePath = (string)RegistryHelper.instance.getValue(RegistryHelper.LOCAL_REPO_CONTAINER_PATH_REG_NAME);
            if (String.IsNullOrEmpty(basePath)) {
                return null;
            }

            return Path.Combine(basePath, repositoryUUID);
        }
    }
}
