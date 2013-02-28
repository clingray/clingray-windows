using System;
using System.Drawing;
using System.Windows.Forms;
using ClingUpdater;
using System.Diagnostics;

namespace ClingClient.forms {
    public partial class frmUpdatePopup : frmCommonPopup {
        UpdateInfo updateInfo = null;
        UpdateDownloader updateDownloader;

        public frmUpdatePopup(UpdateInfo updateInfo) {
            InitializeComponent();

            this.updateInfo = updateInfo;
            version.Text = updateInfo.version;

            showClingLogo = true;
            this.showCloseButton = true;

            hideProgress();

            if (updateInfo.isCritical) {
                Point newPosition = updateButton.Location;
                newPosition.X = skipButton.Right - updateButton.Width;
                updateButton.Location = newPosition;

                skipButton.Hide();

                importantIcon.Show();

                updateTitle.Text = "중요 업데이트가 있습니다.";
            } else {
                Point noticePanelPosition = noticePanel.Location;
                noticePanelPosition.X = importantIcon.Left;
                noticePanel.Location = noticePanelPosition;
                updateTitle.Text = "설치할 업데이트가 있습니다.";
            }
        }

        #region Progress animation related
        private const int BASIC_HEIGHT = 360;
        private const int FULL_HEIGHT = 410;

        private void showProgress() {
            updateButton.Hide();
            skipButton.Hide();
            cancelButton.Show();

            FormTransform transform = new FormTransform();
            transform.TransformSize(this, this.Width, FULL_HEIGHT, onAnimationComplete);
        }

        private void hideProgress() {
            this.Height = BASIC_HEIGHT;
        }

        private void onAnimationComplete() {
            this.Height = FULL_HEIGHT;
            updateDownloader = new UpdateDownloader(updateInfo);
            updateDownloader.downloadUpdate(onDownloadProgress, onDownloadCompletion);
        }
        #endregion

        #region Download related
        public void onDownloadProgress(long bytesReceived, long totalBytesToReceive, int progressPercentage) {
            updateProgress.currentValue = progressPercentage;
        }

        public void onDownloadCompletion(string installerPath) {
            this.Invoke((MethodInvoker)delegate {
                if (installerPath != null)
                {
                    updateProgress.currentValue = 100;

                    Process p = new Process();
                    p.StartInfo.FileName = installerPath;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.CreateNoWindow = true;
                    p.Start();

                    Application.Exit();
                }
                else
                {
                    frmAlertPopup.alert("업데이트 설치에 실패하였습니다.");
                }
            });
        }
        #endregion

        private void frmUpdate_Load(object sender, EventArgs e) {
            this.btnClose.ButtonClick += new EventHandler(closeButton_Click);
            releaseNotes.DocumentText = updateInfo.releaseNotes;
            this.TopMost = true;
        }

        private void updateButton_Click(object sender, EventArgs e) {
            showProgress();
        }

        private void releaseNotes_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
        }

        #region Closing related
        private void skipButton_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Ignore;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            if (updateInfo.isCritical)
            {
                this.DialogResult = DialogResult.Abort;
            }
            else
            {
                this.DialogResult = DialogResult.Ignore;
            }
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e) {
            if (updateInfo.isCritical) {
                this.DialogResult = DialogResult.Abort;
            } else {
                this.DialogResult = DialogResult.Ignore;
            }

            Close();
        }

        private void frmUpdate_FormClosing(object sender, FormClosingEventArgs e) {
            if (updateDownloader != null && updateDownloader.isBusy) {
                updateDownloader.cancel();
            }
        }
        #endregion
    }
}
