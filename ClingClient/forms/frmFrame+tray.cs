using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClientEngine;
using System.Drawing;
using System.Drawing.Imaging;
using ClingClient.common;
using ClingClient.utilities;
using ClingClient.forms;
using ClingClient.subForms;
using ClingUpdater;
using ClingClient.updater;
using ClingClient.popup;

namespace ClingClient
{
    public partial class frmFrame : Form
    {
        private static bool showedHideNotification = false;

        #region Restore from/Send to tray
        public void restoreWindow()
        {
            Show();

            WindowState = FormWindowState.Normal;

            Focus();
            BringToFront();
        }

        public void hideWindow(bool showNotification)
        {
            if (showNotification && !showedHideNotification && !isDisplayingBalloonTip)
            {
                showedHideNotification = true;
                enqueueBalloonTipData(new BalloonTipData(ToolTipIcon.None, "Clingray 가 최소화 되었습니다.",
                    "아이콘을 더블 클릭하여 프로그램을 다시 실행 시킬 수 있습니다."));
            }

            Hide();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            restoreWindow();
        }

        #endregion Restore from/Send to tray

        #region Tray menu
        private void reconstructTrayContextMenu(bool shouldDisplayFullContents)
        {
            // Change visibility.
            currentRepoTrayItem.Visible = shouldDisplayFullContents;
            commitTrayItem.Visible = shouldDisplayFullContents;

            // Refresh recent project list.
            recentRepoTrayItem.DropDown.Items.Clear();

            SortedSet<RepositoryAccessHistoryManager.AccessHistory> histories = RepositoryAccessHistoryManager.instance.historySet;
            foreach (RepositoryAccessHistoryManager.AccessHistory history in histories)
            {
                string repositoryUUID = history.repositoryUUID;
                ClingRepo repository = ClingRayEngine.instance.repositories.getRepository(repositoryUUID);

                if (repository != null)
                {
                    ToolStripMenuItem recentProject = new ToolStripMenuItem();
                    recentProject.Text = repository.name;
                    recentProject.Tag = repositoryUUID;
                    recentProject.Click += new EventHandler(trayItemRecentProject_Click);
                    recentRepoTrayItem.DropDown.Items.Add(recentProject);
                }
            }

            if (shouldDisplayFullContents)
            {
                ClingRepo repository = ClingRayEngine.instance.repositories.getRepository(histories.First().repositoryUUID);

                int MAX_REPO_NAME_IN_MENU = 20;
                string displayedRepoName = StringUtils.getStringWithEllipsis(repository.name, MAX_REPO_NAME_IN_MENU);
                currentRepoTrayItem.Text = string.Format("프로젝트 : {0}", displayedRepoName);
                if (displayedRepoName.EndsWith(StringUtils.ELLIPSIS))
                {
                    currentRepoTrayItem.ToolTipText = repository.name;
                }
            }
        }
        #endregion Tray menu

        #region Tray menu item event
        private void trayItemCurrentRepo_Click(object sender, EventArgs e)
        {
            restoreWindow();
        }

        private void trayItemRecentProject_Click(object sender, EventArgs e)
        {
            restoreWindow();
            if (ApplicationUtils.isDisplayingModalWindow())
            {
                WindowFlasher.Flash(this, 1);
            }
            else
            {
                ToolStripMenuItem item = (ToolStripMenuItem)sender;
                string uuid = (string)item.Tag;

                switchToRepository(uuid);
            }
        }

        private void trayItemCommit_Click(object sender, EventArgs e)
        {
            restoreWindowIfMinimized();

            subCommitList commitList = this.currentSubForm as subCommitList;
            if (commitList != null)
            {
                commitList.commit();
            }
        }

        private void trayItemOpen_Click(object sender, EventArgs e)
        {
            restoreWindow();
        }

        private void trayItemCreateRepo_Click(object sender, EventArgs e)
        {
            restoreWindowIfMinimized();

            if (ApplicationUtils.isDisplayingModalWindow())
            {
                restoreWindow();
                WindowFlasher.Flash(this, 1);
            }
            else
            {
                itemAddProject_Click(sender, e);
            }
        }

        private void trayItemHelp_Click(object sender, EventArgs e)
        {
            itemHelp_Click(sender, e);
        }

        private void trayItemCheckUpdate_Click(object sender, EventArgs e)
        {
            checkUpdate();
        }

        private void checkUpdate()
        {
            restoreWindowIfMinimized();

            Cursor.Current = Cursors.WaitCursor;

            UpdateChecker checker = ClingUpdateHelper.getUpdateChecker();
            UpdateInfo updateInfo = checker.checkUpdate();

            Cursor.Current = Cursors.Default;
            if (updateInfo != null && updateInfo.exists)
            {
                frmUpdatePopup updatePopup = new frmUpdatePopup(updateInfo);
                DialogResult result = updatePopup.ShowDialog(this);
            }
            else
            {
                frmAlertPopup.alert("현재 버전이 가장 최신 버전입니다.");
            }
        }

        private void restoreWindowIfMinimized()
        {
            if (this.WindowState == FormWindowState.Minimized) restoreWindow();
        }

        private void trayItemClingrayWeb_Click(object sender, EventArgs e)
        {
            itemGoWebsite_Click(sender, e);
        }

        private void trayItemSettings_Click(object sender, EventArgs e)
        {
            restoreWindowIfMinimized();
            if (ApplicationUtils.isDisplayingModalWindow())
            {
                restoreWindow();
                WindowFlasher.Flash(this, 1);
            }
            else
            {
                launchConfig();
            }
        }

        private void trayItemExit_Click(object sender, EventArgs e)
        {
            itemQuit_Click(sender, e);
        }

        #endregion Tray menu item event
    }
}
