using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ClingClientEngine;
using System.Windows.Forms;

namespace ClingClient.common
{
    public class WTChangedEventArgs : EventArgs
    {
        public WTChangedEventArgs(ClingRepo repository)
        {
            this.repository = repository;
        }

        public ClingRepo repository { get; set; }
    }

    public class WTMovedEventArgs : EventArgs
    {
        public WTMovedEventArgs(ClingRepo repository, string oldFullPath, string newFullPath)
        {
            this.repository = repository;

            this.oldFullPath = oldFullPath;
            this.newFullPath = newFullPath;
        }

        public ClingRepo repository { get; set; }

        public string oldFullPath { get; set; }
        public string newFullPath { get; set; }
    }

    public delegate void WorkTreeChangeEventHandler(WTChangedEventArgs e);
    public delegate void WorkTreeMoveEventHandler(WTMovedEventArgs e);

    public class WorkTreeMonitoring
    {
        #region Singleton
        static WorkTreeMonitoring _instance = new WorkTreeMonitoring();

        WorkTreeMonitoring()
        {
            workersDictionary = new Dictionary<string, MonitoringWorker>();
        }

        public static WorkTreeMonitoring instance
        {
            get {
                return _instance;
            }
        }
        #endregion

        public static string[] DAW_PROJECT_FILE_FILTERS = { "*.cpr", "*.npr", "*.flp", "*.als", "*.cwp", "*.ptf", "*.pts", "*.rsn", "*.lso", "*.project", "*.song", "*.vsq" };
        private Dictionary<string, MonitoringWorker> workersDictionary;

#region Event handling
        public event WorkTreeChangeEventHandler workTreeChangedHandler;
        public event WorkTreeMoveEventHandler workTreeRenamedHandler;
#endregion Event handling

        public void startMonitoring()
        {
            foreach (ClingRepo repository in ClingRayEngine.instance.repositories.repositories)
            {
                if (repository.hasWorkTree())
                {
                    startMonitoringByRepoId(repository.uuid);
                }
            }
        }

        public bool startMonitoringByWorkTree(string workTreePath)
        {
            ClingRepo repo = ClingRayEngine.instance.repositories.findRepositoryByWorkTree(workTreePath);
            if (repo != null)
            {
                return startMonitoringByRepoId(repo.uuid);
            }

            return false;
        }

        public bool startMonitoringByRepoId(string repositoryUUID)
        {
            if(workersDictionary.ContainsKey(repositoryUUID))
            {
                return false;
            }

            MonitoringWorker worker = MonitoringWorker.createWorker(repositoryUUID, DAW_PROJECT_FILE_FILTERS);
            if (worker != null)
            {
                worker.startMonitoring();
                worker.workTreeChangedHandler += workTreeChangedHandler;
                worker.workTreeRenamedHandler += workTreeRenamedHandler;
                workersDictionary.Add(repositoryUUID, worker);
                return true;
            }

            return false;
        }

        public void stopMonitoring()
        {
            foreach (ClingRepo repository in ClingRayEngine.instance.repositories.repositories)
            {
                stopMonitoringByRepoId(repository.uuid);
            }
        }

        public void stopMonitoringByWorkTree(string workTreePath)
        {
            ClingRepo repo = ClingRayEngine.instance.repositories.findRepositoryByWorkTree(workTreePath);
            if (repo != null)
            {
                stopMonitoringByRepoId(repo.uuid);
            }
        }

        public void stopMonitoringByRepoId(string repositoryUUID)
        {
            if (workersDictionary.ContainsKey(repositoryUUID))
            {
                MonitoringWorker worker = workersDictionary[repositoryUUID];

                worker.workTreeChangedHandler -= workTreeChangedHandler;
                worker.workTreeRenamedHandler -= workTreeRenamedHandler;

                worker.stopMonitoring();                
                workersDictionary.Remove(repositoryUUID);
            }
        }
    }

    internal class MonitoringWorker
    {
        private string repositoryUUID;
        private string[] DAW_PROJECT_FILE_FILTERS;
        List<FileSystemWatcher> watchers;

        #region Event handling
        public WorkTreeChangeEventHandler workTreeChangedHandler;
        public WorkTreeMoveEventHandler workTreeRenamedHandler;
        #endregion Event handling

        #region Construction
        /// <summary>
        /// Factory method
        /// </summary>
        /// <returns>Worker instance or null</returns>
        internal static MonitoringWorker createWorker(string repositoryUUID, string[] DAW_PROJECT_FILE_FILTERS)
        {
            ClingRepo repo = ClingRayEngine.instance.repositories.getRepository(repositoryUUID);
            if (repo.hasWorkTree())
            {
                return new MonitoringWorker(repositoryUUID, DAW_PROJECT_FILE_FILTERS);
            }

            return null;
        }

        private MonitoringWorker(string repositoryUUID, string[] DAW_PROJECT_FILE_FILTERS)
        {
            this.repositoryUUID = repositoryUUID;
            this.DAW_PROJECT_FILE_FILTERS = DAW_PROJECT_FILE_FILTERS;

            watchers = new List<FileSystemWatcher>();
        }
        #endregion Construction

        internal void startMonitoring()
        {
            watchers = startFileSystemWatchers();
        }

        internal void stopMonitoring()
        {
            foreach (FileSystemWatcher watcher in watchers)
            {
                watcher.EnableRaisingEvents = false;
            }
        }

        private List<FileSystemWatcher> startFileSystemWatchers()
        {
            List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();

            ClingRepo repo = ClingRayEngine.instance.repositories.getRepository(repositoryUUID);
            if (repo.hasWorkTree() && Directory.Exists(repo.workTrees[0].path))
            {
                // First watch for rename of working directory.
                string workTreePath = repo.workTrees[0].path;
                FileSystemWatcher watcher = new FileSystemWatcher(Path.GetDirectoryName(workTreePath), Path.GetFileName(workTreePath));

                watcher.NotifyFilter = NotifyFilters.DirectoryName;
                watcher.Renamed += new RenamedEventHandler(onRenameWorkTree);

                watcher.EnableRaisingEvents = true;

                watchers.Add(watcher);

                // And watch for file extensions.
                foreach (string filter in DAW_PROJECT_FILE_FILTERS)
                {
                    watcher = new FileSystemWatcher(repo.workTrees[0].path, filter);

                    watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.CreationTime;
                    watcher.Changed += new FileSystemEventHandler(onChange);
                    watcher.Created += new FileSystemEventHandler(onChange);
                    watcher.Deleted += new FileSystemEventHandler(onChange);

                    // Start watching.
                    watcher.EnableRaisingEvents = true;

                    watchers.Add(watcher);
                }
            }

            return watchers;
        }

        private ClingRepo getRepositoryFromContentPath(string contentPath)
        {
            ClingRepo repository = null;
            string repoCandidate = contentPath;
            do
            {
                repository = ClingRayEngine.instance.repositories.findRepositoryByWorkTree(repoCandidate);

                // Break at root directory. NOT FOUND.
                if (Path.GetPathRoot(contentPath).Equals(repoCandidate))
                {
                    break;
                }

                repoCandidate = Path.GetDirectoryName(repoCandidate);
            } while (repository == null);

            return repository;
        }

        private void onChange(object source, FileSystemEventArgs e)
        {
            if (workTreeChangedHandler != null)
            {
                ClingRepo repository = getRepositoryFromContentPath(e.FullPath);

                if (repository != null)
                {
                    workTreeChangedHandler(new WTChangedEventArgs(repository));
                }
            }
        }

        //void onMoveWorkTree(object sender, FileSystemEventArgs e)
        //{
        //    Console.WriteLine("Type:" + e.ChangeType.ToString());
        //    Console.WriteLine("Path:" + e.FullPath);

        //    ClingRepo repo = ClingRayEngine.instance.repositories.getRepository(repositoryUUID);

        //    applyWorkTreeChange(repo.workTrees[0].path, e.FullPath);
        //}

        private void onRenameWorkTree(object source, RenamedEventArgs e)
        {
            // Specify what is done when a file is renamed.
            Console.WriteLine("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            
            string oldFullPath = e.OldFullPath;
            string newFullPath = e.FullPath;

            applyWorkTreeChange(oldFullPath, newFullPath);
        }

        private void applyWorkTreeChange(string oldFullPath, string newFullPath)
        {
            ClingRepo repository = getRepositoryFromContentPath(oldFullPath);

            if (repository != null)
            {
                stopMonitoring();

                // Update repository information.
                ClingRepo repo = ClingRayEngine.instance.repositories.getRepository(repository.uuid);
                repo.workTrees[0].path = newFullPath;
                ClingRayEngine.instance.saveRepositoryInfo();

                startMonitoring();

                if (workTreeRenamedHandler != null)
                {
                    workTreeRenamedHandler(new WTMovedEventArgs(repository, oldFullPath, newFullPath));
                }
            }
        }
    }
}
