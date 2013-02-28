using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using NGit.Storage.File;
using NGit;
using NGit.Api;
using NGit.Revwalk;
using Sharpen;
using NGit.Treewalk;
using NGit.Dircache;
using NGit.Transport;
using NSch;
using NGit.Api.Errors;
using NGit.Merge;
using NGit.Revplot;

namespace ClingClientEngine {
    public class RepositoryContext {
        public ClingRepo repoInfo { get; set; }

        private RepositoryContext(ClingRepo repository)
        {
            this.repoInfo = repository;
        }

        internal static RepositoryContext createRepository(ClingRepo repoInfo, string authorName, string authorEmail)
        {
            string workTreePath = repoInfo.getWorkTreePath();
            string repositoryPath = RepoUtils.getLocalRepoAbsolutePath(repoInfo.uuid);

            FileRepositoryBuilder builder = new FileRepositoryBuilder();
            Repository repository = builder.SetWorkTree(workTreePath).SetGitDir(repositoryPath).Build();
            repository.Create();
            setAuthorInfo(authorName, authorEmail, repository);

            repository.Close();

            return getContext(repoInfo);
        }

        private static void setAuthorInfo(string authorName, string authorEmail, Repository repository)
        {
            StoredConfig repoConfig = repository.GetConfig();
            if (!string.IsNullOrWhiteSpace(authorName))
            {
                repoConfig.SetString("user", null, "name", authorName);
            }

            if (!string.IsNullOrWhiteSpace(authorEmail))
            {
                repoConfig.SetString("user", null, "email", authorEmail);
            }

            repoConfig.Save();
        }

        public static RepositoryContext getContext(ClingRepo repoInfo) {
            RepositoryContext context = null;

            try
            {
                context = new RepositoryContext(repoInfo);
            }
            catch (Exception)
            {
                Console.WriteLine("Exception during construction of RepositoryContext");
            }

            return context;
        }

        private Repository getRepository()
        {
            Repository repository = null;

            string workTreePath = repoInfo.getWorkTreePath();
            string repositoryPath = RepoUtils.getLocalRepoAbsolutePath(repoInfo.uuid);

            if (!string.IsNullOrWhiteSpace(workTreePath) && Directory.Exists(workTreePath))
            {
                FileRepositoryBuilder builder = new FileRepositoryBuilder();
                repository = builder.SetWorkTree(workTreePath).SetGitDir(repositoryPath).Build();
            }
            else
            {
                FileRepositoryBuilder builder = new FileRepositoryBuilder();
                repository = builder.SetGitDir(repositoryPath).SetBare().Setup().Build();
            }

            setBinaryAttributes(repositoryPath);

            return repository;
        }

        private void setBinaryAttributes(string repositoryPath)
        {
            string infoPath = Path.Combine(repositoryPath, "info");
            if (!Directory.Exists(infoPath) && Directory.Exists(repositoryPath))
            {
                Directory.CreateDirectory(infoPath);
            }

            string attributesPath = Path.Combine(infoPath, @"attributes");

            if (!File.Exists(attributesPath) && Directory.Exists(infoPath))
            {
                string attributes = "* binary";

                using (StreamWriter writer = File.CreateText(attributesPath))
                {
                    writer.WriteLine(attributes);
                    writer.Close();
                }
            }
        }

        // TODO UNSYNCED/SYNCED 알아 내도록 수정.
        public enum Status { UNKNOWN = 0, UNLINKED = 0x01, LINKED = 0x02, CHANGED = 0x04 };
        public Status getStatus() {
            if (repoInfo.dontHaveDotGit()) return Status.UNKNOWN;

            Status retValue = repoInfo.hasWorkTree() ? Status.LINKED : Status.UNLINKED;

            if (isChanged()) retValue |= Status.CHANGED;

            return retValue;
        }

        public bool isChanged() {
            bool isChanged = false;

            Repository repository = getRepository();
            Git git = new Git(repository);

            if (!repository.IsBare)
            {
                isChanged = !git.Status().Call().IsClean();
            }

            repository.Close();

            return isChanged;
        }

        public string getCurrentBranchName()
        {
            Repository repository = getRepository();
            string branchName = repository.GetBranch();
            repository.Close();
            
            return branchName;
        }

        public string getBranchNameByCommitHash(string hash) {
            Repository repository = getRepository();
            RevWalk walk = new RevWalk(repository);
            RevCommit commit = walk.ParseCommit(repository.Resolve(hash + "^0"));

            string branchName = getBranchNameByCommit(commit);

            repository.Close();

            return branchName;
        }

        public string getBranchHeadHash(string branchName)
        {
            Repository repository = getRepository();
            Ref branch = repository.GetRef(branchName);
            string hash = branch.GetObjectId().Abbreviate(40).Name;
            
            repository.Close();
            
            return hash;
        }

        private string getBranchNameByCommit(RevCommit commit) {
            //string branchName = null;

            Repository repository = getRepository();
            //Git git = new Git(repository);

            //try
            //{
            //    ICollection<ReflogEntry> entries = git.Reflog().Call();
            //    foreach (ReflogEntry entry in entries)
            //    {
            //        if (!entry.GetOldId().Name.Equals(commit.Name))
            //        {
            //            continue;
            //        }

            //        CheckoutEntry checkOutEntry = entry.ParseCheckout();
            //        if (checkOutEntry != null)
            //        {
            //            branchName = checkOutEntry.GetFromBranch();
            //            break;
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new Exception("fail to get ref log.", e);
            //}


            RevWalk walk = new RevWalk(repository);

            IDictionary<string, Ref> refs = repository.GetAllRefs();
            foreach (string key in refs.Keys)
            {
                if (key.StartsWith(Constants.R_HEADS))
                {
                    RevCommit tip = walk.ParseCommit(refs[key].GetObjectId());
                    if (walk.IsMergedInto(commit, tip) || commit.Equals(tip))
                    {
                        repository.Close();
                        return refs[key].GetName().Replace(Constants.R_HEADS, string.Empty);
                    }
                }
            }

            repository.Close();
            return null;
        }

        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private string getAbbreviatedHash(RevCommit ngitCommit)
        {
            return ngitCommit.Id.Abbreviate(40).Name;
        }

        private void fillCommitFromNGitCommit(Commit commit, RevCommit ngitCommit)
        {
            string hash = getAbbreviatedHash(ngitCommit);
            DateTime commitDate = UnixTimeStampToDateTime(ngitCommit.CommitTime);
            string comment = ngitCommit.GetFullMessage();
            string authorName = ngitCommit.GetCommitterIdent().GetName();
            string authorEmail = ngitCommit.GetCommitterIdent().GetEmailAddress();
            string branchName = getBranchNameByCommit(ngitCommit);
            string repositoryUUID = repoInfo.uuid;

            commit.hash = hash;
            commit.commitDate = commitDate;
            commit.comment = comment;
            commit.authorName = authorName;
            commit.authorEmail = authorEmail;
            commit.branchName = branchName;
            commit.repositoryUUID = repositoryUUID;
        }

        private Commit getCommitFromNGitCommit(RevCommit ngitCommit) {
            Commit commit = new Commit();
            fillCommitFromNGitCommit(commit, ngitCommit);

            return commit;
        }

        public Commit getLatestCommit()
        {
            Repository repository = getRepository();
            Git git = new Git(repository);

            RevCommit latestCommit = null;
            try
            {
                Iterable<RevCommit> allCommits = git.Log().All().Call();

                foreach (RevCommit commit in allCommits)
                {
                    if (latestCommit == null || commit.CommitTime > latestCommit.CommitTime)
                    {
                        latestCommit = commit;
                    }
                }

            }
            catch (Exception)
            {
                Console.WriteLine("error during retrieval of latest commit.");
            }
            finally
            {
                repository.Close();
            }

            if (latestCommit != null) return getCommitFromNGitCommit(latestCommit);
            
            return null;
        }

        private RevCommit getCurrentNGitCommit()
        {
            if (getVersionCount() == 0) return null;

            Repository repository = getRepository();
            RevWalk revWalk = new RevWalk(repository);
            RevCommit currentNGitCommit = null;
            try
            {
                currentNGitCommit = revWalk.ParseCommit(repository.Resolve(Constants.HEAD));
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Error during getting current commit");
            }
            
            repository.Close();

            return currentNGitCommit;
        }

        public Commit getCurrentCommit() {
            RevCommit headCommit = getCurrentNGitCommit();

            return getCommitFromNGitCommit(headCommit);     
        }

        //stage에 올라가지 않은 파일들 리스트업!
        public string[] loadUntrackedFiles()
        {
            //Console.WriteLine("loadLastCommitFiles:" + libgit2Repo.Index.Count());
            //Console.WriteLine("Missing:" + libgit2Repo.Index.RetrieveStatus().Missing.Count());
            //Console.WriteLine("Modified:" + libgit2Repo.Index.RetrieveStatus().Modified.Count());
            //Console.WriteLine("Removed:" + libgit2Repo.Index.RetrieveStatus().Removed.Count());
            //Console.WriteLine("Added:" + libgit2Repo.Index.RetrieveStatus().Added.Count());
            //Console.WriteLine("Untracked:" + libgit2Repo.Index.RetrieveStatus().Untracked.Count());
            //Console.WriteLine("Staged:" + libgit2Repo.Index.RetrieveStatus().Staged.Count());
            //Console.WriteLine("Ignored:" + libgit2Repo.Index.RetrieveStatus().Ignored.Count());

            //foreach (string name in libgit2Repo.Index.RetrieveStatus().Untracked)
            //{
            //    Console.WriteLine(name);
            //}
            Repository repository = getRepository();

            string[] untrackedFiles = null;
            try
            {
                RevCommit commit = getCurrentNGitCommit();
                WorkingTreeIterator iterator = new FileTreeIterator(repository);

                if (commit != null)
                {
                    IndexDiff diff = new IndexDiff(repository, commit.ToObjectId(), iterator);
                    diff.Diff();
                    untrackedFiles = diff.GetUntracked().ToArray();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (untrackedFiles == null)
                {
                    string workTreePath = repoInfo.getWorkTreePath();
                    untrackedFiles = clsUtil.getAllFileList(workTreePath);
                    // To relative path.
                    for(int i = 0; i < untrackedFiles.Count(); i++)
                    {
                        untrackedFiles[i] = untrackedFiles[i].Substring(workTreePath.Length + 1); // +1 은 \ 를 제외하기 위해.
                    }
                }
                repository.Close();
            }

            return untrackedFiles;
        }

        public StructuralCommit getCommitTree()
        {
            //PlotCommitList<PlotLane> plotCommitList = new PlotCommitList<PlotLane>();

            //Repository repository = getRepository();
            //PlotWalk plotWalk = new PlotWalk(repository);
            //List<Ref> refs = repository.RefDatabase.GetRefs(Constants.R_REFS).Values.ToList();
            //foreach (Ref branch in repository.RefDatabase.GetRefs(Constants.R_REFS).Values)
            //{
            //    if (branch.IsSymbolic())
            //    {
            //        refs.Remove(branch);
            //    }
            //}

            //Sharpen.Iterable<Ref> refsIt = new Sharpen.LinkedHashSet<Ref>(refs.AsEnumerable<Ref>());
            //plotWalk.Sort(RevSort.COMMIT_TIME_DESC);
            //plotWalk.Sort(RevSort.REVERSE);
            //plotWalk.AddAdditionalRefs(refsIt);

            //ObjectId rootId = repository.Resolve("HEAD");

            //if (rootId != null)
            //{
            //    RevCommit rootCommit = plotWalk.ParseCommit(rootId);
            //    plotWalk.MarkStart(rootCommit);
            //    plotCommitList.Source(plotWalk);
            //    plotCommitList.FillTo(Int32.MaxValue);
                
            //    foreach (PlotCommit<PlotLane> lane in plotCommitList)
            //    {
            //        Console.WriteLine("Parent: " + lane.GetFullMessage());
            //        int childCount = lane.GetChildCount();
            //        for (int i = 0; i < childCount; i++)
            //        {
            //            PlotCommit<PlotLane> child = (PlotCommit<PlotLane>)lane.GetChild(i);
            //            Console.WriteLine("\tChild:" + child.GetFullMessage());
            //        }
            //    }
            //}

            Repository repository = getRepository();
            Git git = new Git(repository);

            Iterable<RevCommit> commits = git.Log().All().Call();
            List<RevCommit> commitList = commits.ToList();
            StructuralCommit rootCommit = new StructuralCommit();
            if(commitList.Any())
            {
                RevCommit rootNGitCommit = commitList.Last();
                fillCommitFromNGitCommit(rootCommit, rootNGitCommit);

                for (int i = commitList.Count() - 1; i >= 0; i--)
                {
                    RevCommit ngitCommit = commitList.ElementAt(i);
                    if (ngitCommit.ParentCount > 0)
                    {
                        string parentHash = getAbbreviatedHash(ngitCommit.GetParent(0));
                        StructuralCommit parent = rootCommit.findCommitByHash(parentHash);
                        if (parent != null)
                        {
                            StructuralCommit commit = new StructuralCommit();
                            fillCommitFromNGitCommit(commit, ngitCommit);
                            commit.parent = parent;
                            parent.children.Add(commit);
                        }
                    }
                }
                Debug.WriteLine(rootCommit.ToString());
            }

            repository.Close();

            return rootCommit;
        }

        public List<Commit> getDateOrderedCommits()
        {
            List<Commit> orderedCommits = new List<Commit>();

            Repository repository = getRepository();
            Git git = new Git(repository);

            Iterable<RevCommit> commits = git.Log().All().Call();
            foreach (RevCommit commit in commits)
            {
                orderedCommits.Add(getCommitFromNGitCommit(commit));
            }

            repository.Close();

            return orderedCommits;
        }

        public bool add(String relativePath)
        {
            Repository repository = getRepository();
            Git git = new Git(repository);
            string unixPath = getUnixPath(relativePath);
            DirCache dirCache = git.Add().AddFilepattern(unixPath).Call(); // unix-type path only.
            repository.Close();

            return true;
        }

        private string getUnixPath(string relativePath)
        {
 	        return relativePath.Replace('\\', '/');
        }

        private void setRemote(string remoteURL)
        {
            Repository repository = getRepository();
            StoredConfig config = repository.GetConfig();
            config.SetString("remote", "origin", "url", remoteURL);
            config.SetString("remote", "origin", "fetch", "+refs/heads/*:refs/remotes/origin/*");
            config.Save();

            repository.Close();
        }

        public class CustomJschConfigSessionFactory : JschConfigSessionFactory
        {
            string identityPath { get; set; }
            string knownHostsPath { get; set; }

            public CustomJschConfigSessionFactory()
            {
                this.identityPath = EngineConfig.sshIdentityPath;
                this.knownHostsPath = EngineConfig.sshKnownHostsPath;
            }

            protected override void Configure(OpenSshConfig.Host host, Session session)
            {
                if(!string.IsNullOrWhiteSpace(identityPath))
                {
                    session.jsch.AddIdentity(identityPath);
                }

                if(!string.IsNullOrWhiteSpace(knownHostsPath))
                {
                    session.jsch.SetKnownHosts(knownHostsPath);
                }

                session.SetConfig("StrictHostKeyChecking", "no");
                session.SetConfig("PreferredAuthentications", "publickey");
                session.SetConfig("ServerAliveInterval", "60");
            }
        }

        public delegate void GeneralProgressDelegate(double progressInPercentage);
        class CustomProgressMonitor : ProgressMonitor
        {
            public bool shouldStop { get; set; }

            GeneralProgressDelegate progressDelegate;

            int totalPhases;
            int currentPhase;   // 0-based

            int totalWorksForPhase;
            int currentWorkForPhase;   // 0-based

            double overallProgress;    // 0-100
            double[] progressWeightTable;

            public CustomProgressMonitor(GeneralProgressDelegate progressDelegate, int totalTasks, double[] progressWeightTable)
            {
                this.progressDelegate = progressDelegate;
                
                shouldStop = false;

                this.totalPhases = totalTasks;
                this.currentPhase = -1;

                this.overallProgress = 0;
                this.progressWeightTable = progressWeightTable;
            }

            public override void Start(int totalTasks)
            {
                // totalTasks 를 활용할 수 있다고 생각 했지만,
                // 2013/01/28 현재 그 값이 정확하지 않다.
            }

            public override void BeginTask(string title, int totalWork)
            {
                Console.WriteLine(string.Format("task title: {0}, totalWork: {1}", title, totalWork));
                this.currentPhase++;

                this.totalWorksForPhase = totalWork + 1;
                this.currentWorkForPhase = -1;
            }

            public override void Update(int completed)
            {
                if (progressDelegate != null)
                {
                    this.currentWorkForPhase += completed;

                    double perPhaseProgressLimit = 100f * (progressWeightTable.Length > this.currentPhase ? progressWeightTable[this.currentPhase] : 1);
                    double subProgress = this.totalWorksForPhase == 0 ? 100 : (this.currentWorkForPhase + 1) / (double)this.totalWorksForPhase * 100f;
                    subProgress = Math.Min(100.0, subProgress);

                    double processedProgress = 0;
                    for (int i = 0; i < this.currentPhase; i++)
                    {
                        if(this.currentPhase < this.progressWeightTable.Length) {
                            processedProgress += this.progressWeightTable[i];
                        }
                    }
                    overallProgress = processedProgress * 100 + (subProgress * perPhaseProgressLimit / 100f);
                    Console.WriteLine("----------------------------progress :" + overallProgress);

                    // total task 가 실제 task 수와 맞지 않는다.
                    overallProgress = Math.Min(overallProgress, 100f);
                    
                    progressDelegate(overallProgress);
                }
            }

            public override void EndTask()
            {
                Console.WriteLine("*********** Ending task no:" + currentPhase);
                if (currentPhase == totalPhases - 1 && progressDelegate != null)
                {
                    progressDelegate(100);
                }
            }

            public override bool IsCancelled()
            {
                return shouldStop;
            }
        }

        internal bool cloneRepository(string remoteURL, string workTreePath, string authorName, string authorEmail, RepositoryContext.GeneralProgressDelegate progressDelegate)
        {
            bool result = true;
            CustomJschConfigSessionFactory jschConfigSessionFactory = new CustomJschConfigSessionFactory();
            SshSessionFactory.SetInstance(jschConfigSessionFactory);

            // pull, counting, compressing, receiving, resolving deltas, updating references
            double[] pullWeightTable = { 0.05, 0.05, 0.2, 0.6, 0.05, 0.05 };
            CustomProgressMonitor progressMonitor = new CustomProgressMonitor(progressDelegate, 6, pullWeightTable);
            Git git = null;
            try
            {
                // not a bare repository.
                repoInfo.setWorkTreePath(workTreePath);
                RepositoryContext context = createRepository(repoInfo, authorName, authorEmail);

                if (context == null) throw new Exception("exception during init of repository.");

                // set 'origin'
                setRemote(remoteURL);

                Repository repository = getRepository();
                git = new Git(repository);

                // set author info.
                setAuthorInfo(authorName, authorEmail, repository);

                StoredConfig config = repository.GetConfig();
                // config master branch.
                config.SetString("branch", "master", "remote", "origin");
                config.SetString("branch", "master", "merge", "refs/heads/master");
                // config misc.
                config.SetBoolean("core", null, "logallrefupdates", true);
                config.SetBoolean("core", null, "ignorecase", true);
                config.Save();

                // clear directory.
                if (clsUtil.deleteRecursively(workTreePath))
                {
                    Directory.CreateDirectory(workTreePath);
                    git.Pull().SetProgressMonitor(progressMonitor).Call();
                }
                    
                git.GetRepository().Close();
                Commit latestCommit = getLatestCommit();
                checkoutVersion(latestCommit.hash);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = false;
            }
            finally
            {
                if (git != null)
                {
                    git.GetRepository().Close();
                }
            }

            return result;
        }

        private static bool deleteWorkTreeFolder(string workTreePath)
        {
            if (Directory.Exists(workTreePath))
            {
                return clsUtil.deleteRecursively(workTreePath);
            }

            return true;
        }

        public bool fetch(string remoteURL, GeneralProgressDelegate progressDelegate)
        {
            bool result = true;

            setRemote(remoteURL);

            CustomJschConfigSessionFactory jschConfigSessionFactory = new CustomJschConfigSessionFactory();
            SshSessionFactory.SetInstance(jschConfigSessionFactory);

            // counting, compressing, receiving, resolving deltas, updating delta
            double[] progressWeightTable = { 0.05, 0.05, 0.7, 0.05, 0.05 };
            CustomProgressMonitor progressMonitor = new CustomProgressMonitor(progressDelegate, 5, progressWeightTable);

            Repository repository = getRepository();
            Git git = new Git(repository);

            try
            {
                git.Fetch().SetProgressMonitor(progressMonitor).Call();
            }
            catch (GitAPIException)
            {
                result = false;
            }

            repository.Close();

            return result;
        }

        public bool push(string remoteURL, GeneralProgressDelegate progressDelegate)
        {
            bool result = true;

            setRemote(remoteURL);

            CustomJschConfigSessionFactory jschConfigSessionFactory = new CustomJschConfigSessionFactory();
            SshSessionFactory.SetInstance(jschConfigSessionFactory);

            // Opening connection, Counting objects, Finding sources, Writing
            double[] progressWeightTable = { 0.05, 0.05, 0.05, 0.85 };
            CustomProgressMonitor progressMonitor = new CustomProgressMonitor(progressDelegate, 4, progressWeightTable);

            Repository repository = getRepository();
            Git git = new Git(repository);

            try
            {
                git.Push().SetPushAll().SetProgressMonitor(progressMonitor).Call();
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
                result = false;
            }

            repository.Close();

            return result;
        }

        public bool mergeAll(string remoteURL)
        {
            Commit currentCommit = getCurrentCommit();
            string currentCommitHash = null;
            if (currentCommit != null)
            {
                currentCommitHash = currentCommit.hash;
            }

            Repository repository = getRepository();
            try
            {
                IDictionary<string, Ref> branches = repository.GetAllRefs();
                foreach (string key in branches.Keys)
                {
                    Ref branch = branches[key];
                    if (!branch.IsSymbolic() && branch.GetName().StartsWith(Constants.R_HEADS))
                    {
                        string branchName = branch.GetName().Replace(Constants.R_HEADS, "");

                        checkoutBranch(branchName);

                        BranchTrackingStatus trackingStatus = BranchTrackingStatus.Of(repository, branchName);
                        if (trackingStatus == null) continue;

                        string remoteBranchName = trackingStatus.GetRemoteTrackingBranch();

                        if (!mergeWithRemoteBranch(remoteURL, remoteBranchName))
                        {
                            return false;
                        }
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(currentCommitHash))
                {
                    checkoutVersion(currentCommitHash);
                }

                repository.Close();
            }

            return true;
        }

        public bool mergeWithRemoteBranch(string remoteURL, string branchName)
        {
            bool result = true;

            setRemote(remoteURL);

            Repository repository = getRepository();
            Git git = new Git(repository);

            Ref branch = repository.GetRef(string.Format(branchName));
            MergeCommandResult mergeResult = git.Merge().Include(branch).SetSquash(true).Call();

            if (!mergeResult.GetMergeStatus().IsSuccessful())
            {
                if (mergeResult.GetMergeStatus() == MergeStatus.CONFLICTING)
                {
                    resetMerge();
                    createNewBranch();
                }
                else
                {
                    repository.Close();
                    return false;
                }
            }
            else
            {
                if (mergeResult.GetMergeStatus() == MergeStatus.MERGED_SQUASHED)
                {
                    resetMerge();
                    createNewBranch();
                }
            }

            repository.Close();

            return result;
        }

        public void resetHard()
        {
            reset(ResetCommand.ResetType.HARD);
        }

        public void resetMerge()
        {
            reset(ResetCommand.ResetType.HARD);
        }

        private void reset(ResetCommand.ResetType resetType)
        {
            Repository repository = getRepository();
            Git git = new Git(repository);
            git.Reset().SetMode(resetType).SetRef(Constants.HEAD).Call();

            repository.Close();
        }

        public void createNewBranch()
        {
            Repository repository = getRepository();
            Git git = new Git(repository);

            git.Checkout().SetCreateBranch(true).SetName(getNewBranchName()).Call();

            repository.Close();
        }

        public bool commit(String comment, bool commitWithAdd = true) {
            Console.WriteLine("commit");

            if (isHeadDetached()) createNewBranch();

            Repository repository = getRepository();
            Git git = new Git(repository);
            if (commitWithAdd)
            {
                NGit.Api.Status stat = git.Status().Call();

                ICollection<string> changed = stat.GetChanged();
                ICollection<string> modified = stat.GetModified();
                ICollection<string> deleted = stat.GetRemoved();
                ICollection<string> missing = stat.GetMissing();

                List<string> addedFiles = new List<string>();
                addedFiles.AddRange(changed);
                addedFiles.AddRange(modified);
                addedFiles.AddRange(deleted);
                addedFiles.AddRange(missing);

                if (addedFiles.Count > 0)
                {
                    AddCommand add = git.Add();
                    foreach (string addedFile in addedFiles)
                    {
                        add.AddFilepattern(getUnixPath(addedFile));
                    }

                    add.SetUpdate(true).Call();
                }
            }
            git.Commit().SetAll(true).SetMessage(comment).Call();

            repository.Close();
            
            this.repoInfo.lastModifiedTime = DateTime.UtcNow;

            // TODO noisyblue caller 관계에 있는 클래스를 참조하게 하면 좋지 않다. 
            // repository info 관련 사항은 별도 클래스로 빼야한다.
            ClingRayEngine.instance.saveRepositoryInfo();

            return true;
        }

        private bool isHeadDetached()
        {
            Repository repository = getRepository();
            bool isHeadDetached = ObjectId.IsId(repository.GetBranch());

            repository.Close();

            return isHeadDetached;
        }

        public bool isValidRepository()
        {
            bool result;

            Repository repository = getRepository();
            try
            {
                result = repository.ObjectDatabase.Exists();
                if(result) {
                    result = getCurrentNGitCommit() != null;
                }
            }
            catch (IOException)
            {
                result = false;
            }
            finally
            {
                repository.Close();
            }

            return result;
        }

        private string getNewBranchName()
        {
            return Convert.ToString(DateTime.UtcNow.Ticks);
        }

        public bool checkoutBranch(string branchName)
        {
            Repository repository = getRepository();
            Git git = new Git(repository);

            git.Checkout().SetName(branchName).Call();

            repository.Close();

            return true;
        }

        //변경사항 무시하고 HEAD로 롤백!!!
        public bool rollbackToHEAD()
        {
            try
            {
                resetHard();
                cleanUntracked();

                return true;
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }

            return false;
        }

        public bool checkoutVersion(String commitHash) {
            Console.WriteLine("Checking out " + commitHash);
            bool result = false;

            //마지막 commit 이후에 수정사항 있으면,
            if(isChanged()) rollbackToHEAD();       //현재 branch의 HEAD로 일단 돌려버리고!

            Repository repository = getRepository();
            Git git = new Git(repository);

            //돌아가려는 버젼의 브랜치 알아낸다!
            string destinationBranch = getBranchNameByCommitHash(commitHash);

            //돌아가려는 커밋이 HEAD이면
            if(!string.IsNullOrWhiteSpace(destinationBranch))
            {                
                //브랜치 이름으로 checkout 받아서 HEAD에 연결
                git.Checkout().SetName(destinationBranch).Call();
                result = true;
            }
            else
            {
                //돌아가려는 커밋이 중간에 끼어있는 커밋이면
                //hash로 checkout 받고, head detach 상태로
                git.Checkout().SetName(commitHash).Call();
                result = true;
            }

            repository.Close();

            return result;
        }

        private void deleteEmptySubDirectories(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                deleteEmptySubDirectories(directory);
                if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }

        private void cleanUntracked()
        {
            Repository repository = getRepository();
            Git git = new Git(repository);

            git.Clean().Call();

            // 내용이 지워진 빈 폴더도 지워주지 않아 이와 같이 처리.
            deleteEmptySubDirectories(repository.WorkTree);

            repository.Close();
        }

        public int getVersionCount()
        {
            int versionCount = 0;

            Repository repository = getRepository();
            Git git = new Git(repository);
            try
            {
                Iterable<RevCommit> allCommits = git.Log().All().Call();
                versionCount = allCommits.Count();
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't get version count of " + repoInfo.uuid);
            }

            repository.Close();

            return versionCount;
        }

        //public String showRepoInfo() {
        //    String msg = "";
        //    Console.WriteLine("\nclsGitBiz.showRepoInfo");

        //    try {
        //        msg += "current branch:\t" + libgit2Repo.Head.Name + "\r\n";
        //        msg += "WorkingDirectory:\t" + libgit2Repo.Info.WorkingDirectory + "\r\n";
        //        msg += "Path:\t" + libgit2Repo.Info.Path + "\r\n";
        //        msg += "IsBare:\t" + libgit2Repo.Info.IsBare + "\r\n";
        //        msg += "IsEmpty:\t" + libgit2Repo.Info.IsEmpty + "\r\n";
        //        msg += "IsHeadDetached:\t" + libgit2Repo.Info.IsHeadDetached + "\r\n";
        //        msg += "IsHeadOrphaned:\t" + libgit2Repo.Info.IsHeadOrphaned + "\r\n";

        //        //마지막 commit이후에 변경사항 있는지 여부!
        //        RepositoryStatus status = libgit2Repo.Index.RetrieveStatus();
        //        msg += "status.IsDirty:\t" + status.IsDirty + "\r\n";


        //        msg += "repo.Index.Count:\t" + libgit2Repo.Index.Count + "\r\n";
        //        //addLog("repo.Index.AsQueryable():\t" + repo.Index.

        //        List<IndexEntry> list = libgit2Repo.Index.ToList();
        //        foreach (IndexEntry ee in list) {
        //            msg += (ee.Id + "\t" + ee.Path + "\t" + ee.State + "\t" + ee.Mode + "\r\n");
        //        }
        //    } catch (Exception ee) {
        //        Console.WriteLine(ee.Message);
        //    }

        //    return msg;
        //}



        //internal ArrayList retriveCommitBlob(TreeDefinition td, String parentPath, ProgressIF pIF)
        //{
        //    ArrayList fileList = new ArrayList();

        //    Dictionary<string, TreeEntryDefinition> entries = td.getEntries();
        //    foreach (string key in entries.Keys) {
        //        TreeEntryDefinition _td = entries[key];

        //        if (_td.Type == GitObjectType.Blob) {
        //            Blob blob = libgit2Repo.Lookup<Blob>(_td.TargetId.Sha);

        //            if (blob != null)
        //            {
        //                clsFile file = new clsFile();
        //                file.name = key;
        //                file.path = parentPath;
        //                file.size = blob.Size;

        //                fileList.Add(file);

        //                pIF.sendMsg(file.name, 0);
        //            }

                    
        //        } else {
        //            if (_td.Type == GitObjectType.Tree) {
        //                TreeDefinition td2 = TreeDefinition.From(libgit2Repo.Lookup<Tree>(_td.TargetId.Sha));
        //                ArrayList retList = retriveCommitBlob(td2, parentPath + key + @"/", pIF);

        //                object[] o1 = fileList.ToArray();
        //                object[] o2 = retList.ToArray();
        //                fileList = new ArrayList(o1.Concat(o2).ToArray());
        //            }
        //        }
        //    }

        //    return fileList;
        //}

        //public ArrayList loadCommitFiles(Commit cc, ProgressIF pIF) {
        //    Commit commit = libgit2Repo.Lookup<Commit>(cc.hash);
        //    if (commit != null) {
        //        ArrayList fileList = retriveCommitBlob(TreeDefinition.From(commit.Tree), @"/", pIF);
        //        return fileList;
        //    }

        //    return null;
        //}

        //public Commit loadVersionTree()
        //{
        //    Hashtable rootCommitList = new Hashtable();

        //    Commit rootCommit = null;
        //    Hashtable commitList = new Hashtable();
        //    foreach (Branch _branch in libgit2Repo.Branches)
        //    {
        //        foreach (Commit commit in _branch.Commits)
        //        {
        //            Commit cc = null;
        //            if (commitList.ContainsKey(commit.Sha))
        //            {
        //                cc = (Commit)commitList[commit.Sha];
        //                cc.addBranchName(_branch.Name);
        //            }
        //            else
        //            {
        //                cc = new Commit(commit, repoInfo.uuid, _branch.Name);
        //                commitList.Add(cc.hash, cc);
        //            }

        //            if (_branch.Name.Equals("master") && (commit.ParentsCount == 0)) rootCommit = cc;

        //            foreach (Commit _cc in commit.Parents)
        //            {
        //                Commit ccParent = null;
        //                if (commitList.ContainsKey(_cc.Sha))
        //                {
        //                    ccParent = (Commit)commitList[_cc.Sha];
        //                }
        //                else
        //                {
        //                    ccParent = new Commit(_cc, repoInfo.uuid, _branch.Name);
        //                    commitList.Add(ccParent.hash, ccParent);
        //                }

        //                if (!cc.existCommitInArrayList(ccParent.childCommits)) ccParent.childCommits.Add(cc);
        //                if (!ccParent.existCommitInArrayList(cc.parentCommits)) cc.parentCommits.Add(ccParent);
        //            }
        //        }
        //    }

        //    return rootCommit; //git init만 하고 commit을 한번도 안했으면 null
        //}        
    }
}
