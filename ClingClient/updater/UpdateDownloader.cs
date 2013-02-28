using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.ComponentModel;

namespace ClingUpdater {
    public class UpdateDownloader {
        public delegate void DownloadProgressDelegate(long bytesReceived, long totalBytesToReceive, int progressPercentage);
        public delegate void DownloadCompletionDelegate(string installerPath);

        DownloadProgressDelegate downloadProgressDelegate;
        DownloadCompletionDelegate downloadCompletionDelegate;

        UpdateInfo updateInfo;
        WebClient client;

        public bool isBusy {
            get {
                if (client != null) {
                    return client.IsBusy;
                }

                return false;
            }
        }

        public UpdateDownloader(UpdateInfo updateInfo) {
            this.updateInfo = updateInfo;
        }

        private string getInstallerPath() {
            string installerName = updateInfo.packageFileName;
            string installerPath = Path.Combine(Path.GetTempPath(), installerName);

            return installerPath;
        }

        public void downloadUpdate(DownloadProgressDelegate downloadProgressDelegate,
                                    DownloadCompletionDelegate downloadCompletionDelegate) {
            try {
                client = new WebClient();

                this.downloadProgressDelegate = downloadProgressDelegate;
                this.downloadCompletionDelegate = downloadCompletionDelegate;

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(downloadProgress);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(downloadCompleted);
                client.DownloadFileAsync(new Uri(updateInfo.packageUrl), getInstallerPath());
            } catch (WebException e) {
                Console.WriteLine(e);
            }
        }

        public void cancel() {
            if (client != null) {
                if (client.IsBusy) {
                    client.CancelAsync();
                }
            }
        }

        void downloadProgress(object sender, DownloadProgressChangedEventArgs args) {
            if (downloadProgressDelegate != null) {
                downloadProgressDelegate(args.BytesReceived, args.TotalBytesToReceive, args.ProgressPercentage);
            }
        }

        void downloadCompleted(object sender, AsyncCompletedEventArgs args) {
            if (args.Error == null && !args.Cancelled) {
                if (downloadCompletionDelegate != null) {
                    downloadCompletionDelegate(getInstallerPath());
                } else {
                    downloadCompletionDelegate(null);
                }
            }
        }
    }
}
