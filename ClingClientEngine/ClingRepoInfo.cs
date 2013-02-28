using System.Collections.Generic;
using System.Runtime.Serialization;
using System;
using System.IO;

namespace ClingClientEngine {
    [DataContract(Name = "cling", Namespace = "")]
    [KnownType(typeof(ClingRepo))]
    public class ClingRepositoryData {
        [DataMember]
        public List<ClingRepo> repositories { get; set; }

        [OnDeserializing]
        internal void OnDeserializingCallBack(StreamingContext streamingContext) {
            repositories = new List<ClingRepo>();
        }

        public ClingRepositoryData() {
            repositories = new List<ClingRepo>();
        }

        public ClingRepo getRepository(string repositoryUUID) {
            return repositories.Find(o => o.uuid == repositoryUUID);
        }

        public bool addRepository(ClingRepo repository) {
            if (hasRepository(repository.uuid)) {
                return false;
            }

            repositories.Add(repository);
            return true;
        }

        public bool hasRepository(string repositoryUUID) {
            return getRepository(repositoryUUID) != null;
        }

        public void removeRepository(string repositoryUUID) {
            ClingRepo repository = getRepository(repositoryUUID);
            repositories.Remove(repository);
        }

        public ClingRepo findRepositoryByWorkTree(string workTreePath) {
            foreach (ClingRepo repository in repositories) {
                foreach (ClingWorkTree workDir in repository.workTrees) {
                    if (workDir.path.ToLower().Equals(workTreePath.ToLower())) {
                        return repository;
                    }
                }
            }

            return null;
        }

        public void validateRepositories()
        {
            List<ClingRepo> iterationList = new List<ClingRepo>(repositories);
            foreach (ClingRepo repository in iterationList)
            {
                string repositoryPath = RepoUtils.getLocalRepoAbsolutePath(repository.uuid);
                repository.validateWorkTrees();
            }
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            ClingRepositoryData repoData = obj as ClingRepositoryData;
            if (repoData == null) {
                return false;
            }

            foreach (ClingRepo repo in repositories) {
                if (repoData.repositories.Find(o => o.Equals(repo)) == null) {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode() {
            return repositories.GetHashCode();
        }
    }

    [DataContract(Name="repository", Namespace="")]
    [KnownType(typeof(ClingWorkTree))]
    public class ClingRepo {
        [DataMember]
        public int coverImageIndex { get; set; }
        [DataMember]
        public string uuid { get; set; }
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string relativePath { get; set; }
        [DataMember]
        public List<ClingWorkTree> workTrees { get; set; }
        [DataMember(Name="creationTime")]
        public string creationTimeStr {
            get { return creationTime.Ticks == 0 ? null : creationTime.ToUniversalTime().ToString("o"); }
            set { if (value != null) creationTime = DateTime.Parse(value); }
        }
        [DataMember(Name="lastModifiedTime")]
        public string lastModifiedTimeStr {
            get { return lastModifiedTime.Ticks == 0 ? null : lastModifiedTime.ToUniversalTime().ToString("o"); }
            set { if (value != null) lastModifiedTime = DateTime.Parse(value).ToUniversalTime(); }
        }
        [DataMember(Name="lastSyncTime")]
        public string lastSyncTimeStr {
            get { return lastSyncTime.Ticks == 0 ? null : lastSyncTime.ToUniversalTime().ToString("o"); }
            set { if(value != null) lastSyncTime = DateTime.Parse(value).ToUniversalTime(); }
        }

        public string absolutePath {
            get {
                return RepoUtils.getLocalRepoAbsolutePath(uuid);
            }
        }

        public DateTime creationTime { get; set; }
        public DateTime lastModifiedTime { get; set; }
        public DateTime lastSyncTime { get; set; }

        public ClingRepo() {
            workTrees = new List<ClingWorkTree>();
        }

        public ClingRepo(string name, string uuid, string workTreePath = null) {
            this.name = name;
            this.uuid = uuid;
            this.relativePath = uuid;
            this.workTrees = new List<ClingWorkTree>();

            if (!string.IsNullOrWhiteSpace(workTreePath)) setWorkTreePath(workTreePath);
        }

        public void setWorkTreePath(string workTreePath)
        {
            this.workTrees.Clear();
            this.workTrees.Add(new ClingWorkTree(workTreePath));
        }

        public string getWorkTreePath()
        {
            if (workTrees.Count > 0)
            {
                return workTrees[0].path;
            }

            return null;
        }

        //.git 없는지 여부
        public bool dontHaveDotGit()
        {
            string repoPath = RepoUtils.getLocalRepoAbsolutePath(uuid);

            if( !Directory.Exists(repoPath) ) return true;
            else return false;
        }

        public bool isUnLinkedRepo()
        {
            //.git 만 있는 경우!
            if (!dontHaveDotGit() && !hasWorkTree()) return true;
            else return false;
        }

        public bool hasWorkTree()
        {
            return workTrees.Count > 0;
        }

        public bool hasWorkTree(string workTreePath) {
            if (string.IsNullOrWhiteSpace(getWorkTreePath())) return false;

            return getWorkTreePath().ToLower().Equals(workTreePath.ToLower());
        }

        public void addWorkTree(string workTreePath) {
            workTrees.Add(new ClingWorkTree(workTreePath));
        }

        public void removeWorkTree() {
            workTrees.Clear();
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            ClingRepo repo = obj as ClingRepo;
            if (repo == null) {
                return false;
            }

            foreach(ClingWorkTree workTree in workTrees) {
                if(repo.workTrees.Find(o => o.Equals(workTree)) == null) {
                    return false;
                }
            }

            return uuid.Equals(repo.uuid) &&
                relativePath.Equals(repo.relativePath);
                
        }

        public override int GetHashCode() {
            return uuid.GetHashCode();
        }

        internal void validateWorkTrees()
        {
            List<ClingWorkTree> iterationList = new List<ClingWorkTree>(workTrees);
            foreach (ClingWorkTree workTree in iterationList)
            {
                string workTreePath = workTree.path;
                if (dontHaveDotGit() || !Directory.Exists(workTreePath))
                {
                    removeWorkTree();
                }
            }
        }
    }

    [DataContract(Name = "workTree", Namespace="")]
    public class ClingWorkTree {
        [DataMember]
        public string path { get; set; }

        public ClingWorkTree(string path) {
            this.path = path;
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }

            ClingWorkTree workTree = obj as ClingWorkTree;
            if (workTree == null) {
                return false;
            }

            if (path == null) {
                return path == workTree.path;
            }

            return path.Equals(workTree.path);
        }

        public override int GetHashCode() {
            if (path == null) {
                return 0;
            }

            return path.GetHashCode();
        }

        public override string ToString() {
            return path;
        }
    }
}
