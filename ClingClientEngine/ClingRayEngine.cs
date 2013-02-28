using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ClingClientTest")]
namespace ClingClientEngine {
    public class ClingRayEngine {
        EngineConfig config = new EngineConfig();

        #region Singleton
        static ClingRayEngine _instance = new ClingRayEngine();

        ClingRayEngine() {
        }

        public static ClingRayEngine instance {
            get {
                return _instance;
            }
        }
        #endregion

        #region Initialization/Uninitialization
        public bool isInitialized { get; set; }

        /// <summary>
        /// Engine 을 초기화 하고 meta data 를 load 한다.
        /// </summary>
        /// <param name="config">Config</param>
        /// <returns></returns>
        public bool initialize() {
            if (!string.IsNullOrWhiteSpace(EngineConfig.baseRegistryKeyName)) RegistryHelper.instance.baseRegistryKeyName = EngineConfig.baseRegistryKeyName;
            string repositoryContainerPath = (string)RegistryHelper.instance.getValue(RegistryHelper.LOCAL_REPO_CONTAINER_PATH_REG_NAME);

            if (!string.IsNullOrWhiteSpace(EngineConfig.repositoryContainerPath)) repositoryContainerPath = EngineConfig.repositoryContainerPath;

            isInitialized = false;

            if (String.IsNullOrEmpty(EngineConfig.baseRegistryKeyName) ||
                String.IsNullOrEmpty(repositoryContainerPath)) {
                    throw new ArgumentException("baseRegistryKeyName and repositoryContainerPath variables must be set!");
            }

            Console.WriteLine("[ClingRayEngine] Version : " + getVersion());

            if (string.IsNullOrWhiteSpace(repositoryContainerPath) ||
                !string.IsNullOrWhiteSpace(EngineConfig.repositoryContainerPath))
            {
                RegistryHelper.instance.setValue(RegistryHelper.LOCAL_REPO_CONTAINER_PATH_REG_NAME, EngineConfig.repositoryContainerPath);
            }

            if (!loadRepositories()) {
                return false;
            }

            isInitialized = true;
            return true;
        }

        internal string getVersion() {
            return this.GetType().Assembly.GetName().Version.ToString();
        }

        /// <summary>
        /// Engine 에서 사용하던 리소스를 해제 한다.
        /// </summary>
        public void uninitialize() {
        }
        #endregion

        #region Repositories meta data handling
        /// <summary>
        /// Local repository 들을 가지는 container directory 의 path.
        /// </summary>
        public string repositoryContainerPath {
            get {
                return (string)RegistryHelper.instance.getValue(RegistryHelper.LOCAL_REPO_CONTAINER_PATH_REG_NAME);
            }
        }

        internal const string REPOSITORIES_META_FILENAME = @"repositories.xml";
        internal string getRepositoriesMetaPath() {
            if (!string.IsNullOrWhiteSpace(repositoryContainerPath))
            {
                return Path.Combine(repositoryContainerPath, REPOSITORIES_META_FILENAME);
            }
            else
            {
                return null;
            }
        }

        internal long myRepositoryInfoTimestamp = -1;
        internal ClingRepositoryData _repositories = new ClingRepositoryData();  // loadRepositoryInfo, saveRepositoryInfo 에서만 직접 접근한다.
                                                                            // 그 외에는 타 프로세스와의 동기화 문제로 repositoryInfo 를 통하여 접근하도록 한다.
        public ClingRepositoryData repositories {
            // 타 프로세스에 의해 repository 정보가 업데이트 되는 경우 
            // 프로세스 간 repository 정보 동기화 문제를 위해 getter 에서 update timestamp 를 체크한다.
            // 이 방법은 매번 polling 을 하므로 효율적이지 못하다.
            // 추후 업데이트 될 시에 notify 되는 방식이나 shared memory 를 사용할 수 있도록 한다.
            get {
                string lastTimestampStr = (string)RegistryHelper.instance.getValue(RegistryHelper.REPOSITORY_INFO_TIMESTAMP);

                if (lastTimestampStr != null) {
                    long lastTimestamp = long.Parse(lastTimestampStr);

                    if (lastTimestamp > myRepositoryInfoTimestamp) {
                        loadRepositories();
                    }
                } else {
                    // Force load on initial start.
                    RegistryHelper.instance.setValue(RegistryHelper.REPOSITORY_INFO_TIMESTAMP, DateTime.UtcNow.Ticks);

                    loadRepositories();
                }

                return _repositories;
            }
        }

        internal bool loadRepositories() {
            bool succeeded = true;

            FileStream stream = null;
            try {
                if (File.Exists(getRepositoriesMetaPath()))
                {
                    stream = new FileStream(getRepositoriesMetaPath(), FileMode.Open, FileAccess.Read);

                    DataContractSerializer serializer = new DataContractSerializer(typeof(ClingRepositoryData));
                    _repositories = (ClingRepositoryData)serializer.ReadObject(stream);

                    myRepositoryInfoTimestamp = DateTime.UtcNow.Ticks;
                }
                else
                {
                    _repositories.repositories.Clear();
                    myRepositoryInfoTimestamp = -1;
                }
            } catch (XmlException) {
                Console.WriteLine("repository info couldn't be loaded due to XmlException.");
            } catch (SerializationException) {
                Console.WriteLine("repository info couldn't be loaded due to SerializationException.");
                succeeded = false;
            } finally {
                if (stream != null) {
                    stream.Close();
                }
            }

            _repositories.validateRepositories();

            return succeeded;
        }

        public bool saveRepositoryInfo() {
            bool succeeded = true;

            FileStream stream = null;
            try {
                _repositories.validateRepositories();

                if (!string.IsNullOrWhiteSpace(repositoryContainerPath)
                    && !Directory.Exists(repositoryContainerPath))
                {
                    Directory.CreateDirectory(repositoryContainerPath);
                }

                stream = new FileStream(getRepositoriesMetaPath(), FileMode.Create, FileAccess.Write);

                DataContractSerializer serializer = new DataContractSerializer(typeof(ClingRepositoryData));
                serializer.WriteObject(stream, _repositories);

                myRepositoryInfoTimestamp = DateTime.UtcNow.Ticks;
                RegistryHelper.instance.setValue(RegistryHelper.REPOSITORY_INFO_TIMESTAMP, myRepositoryInfoTimestamp);
            } catch (IOException e) {
                Console.WriteLine(e);
                succeeded = false;
            } catch (SerializationException e) {
                Console.WriteLine(e);
                succeeded = false;
            } finally {
                if (stream != null) {
                    stream.Close();
                }
            }

            return succeeded;
        }
        #endregion

        #region Repository management
        /// <summary>
        /// Cling repository 를 생성한다.
        /// </summary>
        /// <param name="repositoryName">생성할 repository 의 이름</param>
        /// <param name="workTreePath">repository 와 mapping 되는 work tree path</param>
        /// <param name="coverImageIndex">repository 에 할당 되는 cover image 번호</param>
        /// <returns>생성된 repository 의 work tree 에 대한 context</returns>
        public RepositoryContext addRepository(string repositoryName, string workTreePath, int coverImageIndex) {

            if (!string.IsNullOrWhiteSpace(workTreePath))
            {
                if (repositories.findRepositoryByWorkTree(workTreePath) != null)
                {
                    return null;
                }
            }

            ClingRepo repository = new ClingRepo(repositoryName, Guid.NewGuid().ToString(), workTreePath);
            repository.creationTime = DateTime.UtcNow;
            repository.lastModifiedTime = repository.creationTime;
            repository.coverImageIndex = coverImageIndex;

            RepositoryContext context = RepositoryContext.createRepository(repository,
                                                                EngineConfig.authorName, EngineConfig.authorEmail);

            if (context != null || string.IsNullOrWhiteSpace(workTreePath)) {
                repositories.repositories.Add(repository);
                saveRepositoryInfo();
            }
       
            return context;
        }

        /// <summary>
        /// Working directory 없이 Cling repository 를 추가한다.
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <param name="coverImageIndex"></param>
        /// <returns>생성된 repository information instance</returns>
        public ClingRepo addUnlinkedRepository(string repositoryUUID, string repositoryName, int coverImageIndex)
        {
            ClingRepo repository = new ClingRepo(repositoryName, repositoryUUID);
            repository.creationTime = DateTime.UtcNow;
            repository.lastModifiedTime = repository.creationTime;
            repository.coverImageIndex = coverImageIndex;

            repositories.repositories.Add(repository);
            saveRepositoryInfo();

            return repository;
        }
        
        /// <summary>
        /// Repository 의 이름을 변경할 때 사용한다.
        /// </summary>
        /// <param name="repositoryUUID">이름을 변경할 repository 의 uuid</param>
        /// <param name="newName">새로운 repository 이름</param>
        public void renameRepository(string repositoryUUID, string newName)
        {
            ClingRepo repo = repositories.getRepository(repositoryUUID);
            if (repo != null)
            {
                repo.name = newName;
                saveRepositoryInfo();
            }
        }

        /// <summary>
        /// Cling repository 를 repository container 에서 제거한다.
        /// 해당 repository 에 연결된 모든 work tree 들도 연결이 해제 된다.
        /// </summary>
        /// <param name="repositoryUUID">제거할 repository UUID</param>
        public void removeRepository(string repositoryUUID) {
            ClingRepo repo = repositories.getRepository(repositoryUUID);
            if (repo != null) {
                string localRepoPath = RepoUtils.getLocalRepoAbsolutePath(repositoryUUID);
                if(Directory.Exists(localRepoPath)) {
                    clsUtil.deleteRecursively(new DirectoryInfo(localRepoPath));
                }

                repositories.removeRepository(repositoryUUID);
                saveRepositoryInfo();
            }
        }

        /// <summary>
        /// 특정 work tree 를 특정 repository 에 연결한다.
        /// 현재는 1:1 연결만 허용하므로 이미 연결된 work tree 가 있을 경우 연결되지 못 한다.
        /// </summary>
        /// <param name="repositoryUUID">연결할 repository UUID</param>
        /// <param name="workTreePath">연결할 work tree path</param>
        /// <returns>boolean</returns>
        public bool linkWorkTree(string repositoryUUID, string workTreePath) {
            if (!Directory.Exists(workTreePath) ||
                repositories.findRepositoryByWorkTree(workTreePath) != null) {
                return false;
            }

            ClingRepo repo = repositories.getRepository(repositoryUUID);
            if (repo != null) {
                if (repo.hasWorkTree(workTreePath)) {
                    return false;
                } else {
                    repo.setWorkTreePath(workTreePath);
                    saveRepositoryInfo();
                }
            }

            return true;
        }

        /// <summary>
        /// 특정 Work tree 를 연결된 repository 에서 연결 해제 한다.
        /// work tree 가 어디에도 연결이 되어 있지 않다면 아무 동작도 하지 않는다.
        /// </summary>
        /// <param name="workTreePath">연결 해제할 work tree 의 path</param>
        public void unlinkWorkTree(string workTreePath) {
            ClingRepo repo = repositories.findRepositoryByWorkTree(workTreePath);
            if (repo != null) {
                repo.removeWorkTree();
                saveRepositoryInfo();
            }
        }
        #endregion

        #region RepositoryContext

        /// <summary>
        /// Work tree 와 연결된 repository 의 context 를 얻는다.
        /// 얻어진 context 로 버전 생성, 버전 되돌리기 등의 동작을 수행 할 수 있다.
        /// </summary>
        /// <param name="workTreePath">work tree path</param>
        /// <returns>RepositoryContext</returns>
        public RepositoryContext getRepositoryContext(string workTreePath) {
            ClingRepo repo = repositories.findRepositoryByWorkTree(workTreePath);
            if (repo != null && Directory.Exists(workTreePath)) {
                return RepositoryContext.getContext(repo);
            }

            return null;
        }

        public RepositoryContext getRepositoryContext(ClingRepo repository)
        {
            return RepositoryContext.getContext(repository);
        }

        public RepositoryContext.Status getWorkTreeStatus(ClingRepo repository)
        {
            if (repository != null && repository.hasWorkTree())
            {
                return getWorkTreeStatus(repository.workTrees[0].path);
            }

            return RepositoryContext.Status.UNKNOWN;
        }

        public RepositoryContext.Status getWorkTreeStatus(string workTreePath) {
            RepositoryContext context = getRepositoryContext(workTreePath);

            if (context == null) {
                return RepositoryContext.Status.UNKNOWN;
            } else {
                return context.getStatus();
            }
        }
        #endregion

        /// <summary>
        /// 원격 repository 를 local 에 clone 시 사용한다.
        /// </summary>
        /// <param name="remoteURL"></param>
        /// <param name="repositoryUUID"></param>
        /// <param name="workTreePath"></param>
        /// <param name="progressDelegate"></param>
        /// <returns></returns>
        public RepositoryContext cloneRepository(string remoteURL, string repositoryUUID, string workTreePath, RepositoryContext.GeneralProgressDelegate progressDelegate)
        {
            ClingRepo repoInfo = repositories.getRepository(repositoryUUID);
            RepositoryContext context = getRepositoryContext(repoInfo);

            bool succeeded = context.cloneRepository(remoteURL, workTreePath, EngineConfig.authorName, EngineConfig.authorEmail, progressDelegate);
            if (succeeded)
            {
                linkWorkTree(repositoryUUID, workTreePath);
                saveRepositoryInfo();

                return getRepositoryContext(repoInfo);
            }

            return null;
        }

        public void cancelCurrentLongRunningTask()
        {

        }

        private bool canSetDirectoryWithWorkDirs(string directory)
        {
            // check if new/existing work tree contains each other.
            foreach (ClingRepo repoInfo in repositories.repositories)
            {
                string existingWorkTree = repoInfo.getWorkTreePath();
                if (!string.IsNullOrWhiteSpace(existingWorkTree))
                {
                    if (clsUtil.directoryContainsAnotherDirectory(existingWorkTree, directory)
                    || clsUtil.directoryContainsAnotherDirectory(directory, existingWorkTree))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool canSetBackupDirectory(string backupDirectory)
        {
            // check if root directory.
            if (Path.GetFullPath(backupDirectory).Equals(Path.GetPathRoot(backupDirectory)))
                return false;

            return canSetDirectoryWithWorkDirs(backupDirectory);
        }

        public bool canSetWorkingFolder(string workTreePath)
        {
            // check if backup directory or work tree contains other.
            string backupPath = (string)RegistryHelper.instance.getValue(RegistryHelper.LOCAL_REPO_CONTAINER_PATH_REG_NAME);
            if (clsUtil.directoryContainsAnotherDirectory(backupPath, workTreePath)
                || clsUtil.directoryContainsAnotherDirectory(workTreePath, backupPath))
            {
                return false;
            }

            if (!canSetDirectoryWithWorkDirs(workTreePath))
            {
                return false;
            }

            // check if root directory.
            if (Path.GetFullPath(workTreePath).Equals(Path.GetPathRoot(workTreePath)))
                return false;

            return true;
        }
    }
}
