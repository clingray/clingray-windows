using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.common;
using ClingClientEngine;
using ClingClient.forms;
using ClingClient.utilities;
using ClingClient.popup;
using ClingClient.subForms;

namespace ClingClient
{
    public partial class frmFrame : Form
    {
        private void startRepositoryMonitoring()
        {
            // To avoid duplicate registration.
            WorkTreeMonitoring.instance.workTreeChangedHandler -= onWorkTreeChanged;
            WorkTreeMonitoring.instance.workTreeChangedHandler += onWorkTreeChanged;

            WorkTreeMonitoring.instance.startMonitoring();
        }

        private void stopRepositoryMonitoring()
        {
            WorkTreeMonitoring.instance.workTreeChangedHandler -= onWorkTreeChanged;
            WorkTreeMonitoring.instance.stopMonitoring();
        }

        void onWorkTreeRenamed(WTMovedEventArgs e)
        {
        }

        void onWorkTreeChanged(WTChangedEventArgs e)
        {
            ClingRepo changedRepository = e.repository;

            RepositoryContext.Status status = ClingRayEngine.instance.getWorkTreeStatus(changedRepository);
            if ((status & RepositoryContext.Status.CHANGED) != 0)
            {
                refreshProjectListOnUIThreadIfPossible();

                showChangedBalloonTip(changedRepository);
            }
        }

        private void refreshCurrentSubForm()
        {
            subCommitList commitList = this.currentSubForm as subCommitList;
            if (commitList != null)
            {
                commitList.refreshAll();
            }
            else
            {
                refreshProjectListOnUIThreadIfPossible();
            }
        }

        private void refreshProjectListOnUIThreadIfPossible()
        {
            subProjectList projectList = currentSubForm as subProjectList;
            if (projectList != null)
            {
                Action action = () => projectList.refreshProjectList();
                Invoke(action);
            }
        }

        void onVerifyCommitFromBallonClicked(object sender, EventArgs e)
        {
            restoreWindow();

            if (currentBalloonTipData != null)
            {
                ClingRepo repository = currentBalloonTipData.tag as ClingRepo;
                if (repository != null) showCommitList(repository);
            }
        }

        void showChangedBalloonTip(ClingRepo repository)
        {
            // Skip on previous balloon tip.
            if (isDisplayingBalloonTip && 
                currentBalloonTipData != null && 
                currentBalloonTipData.tag is ClingRepo)
            {
                return;
            }

            int MAX_REPO_NAME_IN_BALLOON = 20;
            string displayedRepoName = StringUtils.getStringWithEllipsis(repository.name, MAX_REPO_NAME_IN_BALLOON);

            string title = string.Format("{0} 프로젝트가 변경되었습니다.", displayedRepoName);
            string text = "여기를 클릭하면 작업 기록을 남길 수 있습니다.";

            BalloonTipData balloonTipData = new BalloonTipData(ToolTipIcon.Info, title, text, onCommitFromBallonClicked);
            balloonTipData.tag = repository;

            showBalloonTipImmediately(balloonTipData);
        }

        void onCommitFromBallonClicked(object sender, EventArgs e)
        {
            SharedPopupController.UIUpdateDelegate onSuccessfulCommit = result =>
            {
                if (result == SharedPopupController.Result.Success)
                {
                    refreshCurrentSubForm();

                    BalloonTipData balloonTipData = new BalloonTipData(ToolTipIcon.Info, "작업기록 완료!", 
                        "여기를 클릭하면 기록 내역을 확인할 수 있습니다.", onVerifyCommitFromBallonClicked);
                    balloonTipData.tag = currentBalloonTipData.tag;
                    showBalloonTipImmediately(balloonTipData);
                }
                else if (result == SharedPopupController.Result.Error)
                {
                    BalloonTipData balloonTipData = new BalloonTipData(ToolTipIcon.Error,
                                    "작업기록 실패!", "메뉴를 통해 다시 한 번 시도해 주세요.");
                    showBalloonTipImmediately(balloonTipData);
                }
            };

            ClingRepo repository = (ClingRepo)currentBalloonTipData.tag;
            RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(repository);

            restoreWindowIfMinimized();

            showCommitList(repository);
            SharedPopupController.showCommitDialog(context, this.currentSubForm as IWin32Window, onSuccessfulCommit);
        }
    }
}
