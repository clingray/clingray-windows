using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClingClient.controls;
using ClingClientEngine;
using ClingClient.forms;
using ClingClient.commitList;
using ClingClient.utilities;
using ClingClient.popup;
using ClingClient.Properties;
using ClingClient.common;
using System.IO;

namespace ClingClient.subForms {
    public partial class subCommitList : ClingControlBase, subFormInterface
    {
        Bitmap[] CDJacketForVersionList = new Bitmap[] { 
            Resources.img_list_disk_01,
            Resources.img_list_disk_02,
            Resources.img_list_disk_03,
            Resources.img_list_disk_04,
            Resources.img_list_disk_05,
            Resources.img_list_disk_06,
            Resources.img_list_disk_07,
            Resources.img_list_disk_08,
            Resources.img_list_disk_09,
            Resources.img_list_disk_10
        };

        public string repositoryUUID { get; set; }

        private const int PAGE_LENGTH = 100;
        private const string EMPTY_TIMESTAMP = "-";
        private const string TIMESTAMP_RENDER_FORMAT = "yyyy-MM-dd HH:mm";
        private const string CREATION_TIMESTAMP_RENDER_FORMAT = "yyyy-MM-dd HH:mm";

        private RepositoryContext context;
        private int currentPage = 0;
        private List<Commit> allCommits;
        private bool isReadOnly = false;

        public subCommitList() {
            InitializeComponent();

            this.Resize += new EventHandler(subCommitList_Resize);
            this.repoStatusView.localStatusButton.ButtonClick += new EventHandler(localStatusButton_ButtonClick);
        }

        void subCommitList_Resize(object sender, EventArgs e)
        {
            tableView.Size = new Size(this.Width - tableView.Left - 24, this.Height - tableView.Top);
        }

        void localStatusButton_ButtonClick(object sender, EventArgs e)
        {
            bool shouldDeleteWorkTree;
            string workTreePath = SharedPopupController.showSelectNewWorkTree(this, out shouldDeleteWorkTree);

            if (!string.IsNullOrWhiteSpace(workTreePath))
            {
                ClingRayEngine.instance.linkWorkTree(context.repoInfo.uuid, workTreePath);
                if (shouldDeleteWorkTree)
                {
                    context.checkoutVersion(context.getLatestCommit().hash);
                }
                
                refreshAll();
            }
        }

        public void refreshAll(bool updatePageToCurrentCommit = false, bool scrollToCurrentCommit = false) {
            Cursor.Current = Cursors.WaitCursor;
            SuspendLayout();
            refreshContext();

            if (!refreshCommitData(updatePageToCurrentCommit))
            {
                frmAlertPopup.alert("정상적인 프로젝트가 아닙니다.\n프로젝트를 삭제 후 다시 생성해 주시기 바랍니다.");
                goBack();
                return;
            }

            refreshTimestamps();
            refreshStatusViews();
            showCommitList(0, currentPage, scrollToCurrentCommit);
            
            ResumeLayout();
            Cursor.Current = Cursors.Default;
        }

        private void refreshStatusViews() {
            int versionCount = context.getVersionCount();
            string repoSize = SystemUtils.getFolderSizeAutoUnit(context.repoInfo.absolutePath, 1);
            string repoStatus = String.Format("총 {0}개 버전 / {1}", versionCount, repoSize);

            RepositoryContext.Status status = new RepoHelper().getTotalRepoStatus(context.repoInfo);

            this.repoStatusView.setRepositoryStatus(status);
            toolBelt.repositoryStatus = repoStatus;

            // commit button.
            if (status.HasFlag(RepositoryContext.Status.UNLINKED))
            {
                commitButton.bmpNormal = Resources.btn_write_record_disable;
                commitButton.Enabled = false;

                isReadOnly = true;
            } else
            {
                commitButton.bmpNormal = Resources.btn_write_record_normal;
                commitButton.Enabled = true;

                isReadOnly = false;
            }
        }

        Bitmap getCoverImage(ClingRepo repo)
        {
            //인덱스 체크
            int idx = ((repo.coverImageIndex + 1) > CDJacketForVersionList.Length) ? 0 : repo.coverImageIndex;
            return CDJacketForVersionList[idx];
        }

        private void refreshContext() {
            ClingRepo repository = ClingRayEngine.instance.repositories.getRepository(repositoryUUID);
            context = ClingRayEngine.instance.getRepositoryContext(repository);

            //앨범 커버 이미지
            coverBG.Image = getCoverImage(repository);
        }

        private void refreshTimestamps() {
            DateTime lastModifiedTime = context.repoInfo.lastModifiedTime;
            DateTime lastSyncTime = context.repoInfo.lastSyncTime;
            DateTime projectCreationTime = context.repoInfo.creationTime;

            if (lastModifiedTime.Ticks == 0) {
                lastModifiedLabel.Text = EMPTY_TIMESTAMP;
            } else {
                lastModifiedLabel.Text = lastModifiedTime.ToLocalTime().ToString(TIMESTAMP_RENDER_FORMAT);
            }

            if (projectCreationTime.Ticks == 0) {
                creationLabel.Text = EMPTY_TIMESTAMP;
            } else {
                creationLabel.Text = projectCreationTime.ToLocalTime().ToString(CREATION_TIMESTAMP_RENDER_FORMAT);
            }
        }

        private bool refreshCommitData(bool updatePageToCurrentCommit = false) {
            if (context == null) return false;
            if (!context.isValidRepository()) return false;

            allCommits = context.getDateOrderedCommits();

            if(updatePageToCurrentCommit)
            {   
                Commit currentCommit = context.getCurrentCommit();
                for (int idx = 0; idx < allCommits.Count; idx++)
                {
                    if (allCommits[idx].hash == currentCommit.hash)
                    {
                        currentPage = idx / PAGE_LENGTH;
                        break;
                    }
                }
            }

            return true;
        }

        void showMore() {
            Cursor.Current = Cursors.WaitCursor;

            currentPage++;
            showCommitList(0, currentPage);

            Cursor.Current = Cursors.Default;
        }

        void showCommitList(int startPage, int endPage, bool scrollToCurrentCommit = false) {
            int startIndex = startPage * PAGE_LENGTH;
            int endIndex = Math.Min((endPage + 1) * PAGE_LENGTH - 1, allCommits.Count - 1);
            bool shouldRefresh = startPage == 0;

            Commit currentCommit = context.getCurrentCommit();
            DateTime comparedTime;
            int currentCommitCellIndex = -1;

            if (currentCommit == null)
            {
                return;
            }

            //툴팁 memory leck 방지를 위한 수동 dispose 
            ToolTipHelper.removeFromControl(tableView);     

            if (shouldRefresh) {
                comparedTime = allCommits[startIndex].commitDate;
                tableView.itemList.Clear();
            } else {
                comparedTime = allCommits[startIndex - 1].commitDate;
                // Remove separator.
                tableView.itemList.Remove(tableView.itemList.Last());
            }

            for (int idx = startIndex; idx <= endIndex; idx++) {
                Commit commit = allCommits[idx];
                CommitListCell cell = createCellFromCommit(commit, commit.hash == currentCommit.hash);
                if (cell.isCurrentVersion)
                {
                    currentCommitCellIndex = idx;
                }

                int commitYear = commit.commitDate.Year;
                int commitMonth = commit.commitDate.Month;
                if (commitMonth != comparedTime.Month || commitYear != comparedTime.Year) {
                    MonthSeparatorBar separator = new MonthSeparatorBar(comparedTime);
                    tableView.itemList.Add(separator);

                    comparedTime = commit.commitDate;
                }

                cell.viewFilesButton.Click += new EventHandler(viewFilesButton_Click);
                cell.checkoutButton.Click += new EventHandler(checkoutButton_Click);
                tableView.itemList.Add(cell);
            }
            
            if (endIndex < allCommits.Count - 1)
            {
                ClingButton showMoreButton = new ClingClient.ClingButton();
                showMoreButton.Anchor = AnchorStyles.Bottom;
                showMoreButton.BackColor = Color.Transparent;
                showMoreButton.BackgroundImage = Resources.btn_list_more;
                showMoreButton.BackgroundImageLayout = ImageLayout.None;
                showMoreButton.bmpIn = null;
                showMoreButton.bmpNormal = Resources.btn_list_more;
                showMoreButton.bmpPress = null;
                showMoreButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
                showMoreButton.Name = "showMoreButton";
                showMoreButton.Size = new System.Drawing.Size(445, 37);
                showMoreButton.Click += new System.EventHandler(showMoreButton_Click);

                tableView.itemList.Add(showMoreButton);
            }
            else
            {
                // Add last separator.
                MonthSeparatorBar lastSeparator = new MonthSeparatorBar(allCommits.Last().commitDate);
                tableView.itemList.Add(lastSeparator);
            }

            tableView.refreshItems();

            if (scrollToCurrentCommit && currentCommitCellIndex != -1)
            {
                int scrollIndex = Math.Min(tableView.itemList.Count - 1, currentCommitCellIndex + 2);
                tableView.ScrollControlIntoView(tableView.itemList[scrollIndex]);
                //Console.WriteLine("max: {0}, min: {1}, current : {2}", tableView.VerticalScroll.Maximum, tableView.VerticalScroll.Minimum, currentCommitCell.Location.Y);
            }

            GC.Collect();
        }

        void viewFilesButton_Click(object sender, EventArgs e)
        {
            ClingButton button = (ClingButton)sender;
            CommitListCell cell = (CommitListCell)button.Parent;

            showFiles(cell);
        }

        void checkoutButton_Click(object sender, EventArgs e) {
            if (clsUtil.isDirectoryInUse(context.repoInfo.getWorkTreePath()))
            {
                frmAlertPopup.alert("작업 폴더가 다른 프로그램에 의해 사용 중입니다.\n다른 프로그램을 종료 후 다시 시도해 주세요.");
                return;
            }

            ClingButton button = (ClingButton)sender;
            CommitListCell cell = (CommitListCell)button.Parent;

            checkout(cell);
        }

        private void checkout(CommitListCell cell) {
            Size popupSize = new Size(425, 259);
            string alertTitle, alertMessage;
            frmAlertPopup alert;
            DialogResult result;

            string ellipsizedProjectName = StringUtils.getStringWithEllipsis(context.repoInfo.name, 15);

            alertTitle = "지금 프로젝트를 닫아주세요!";
            alertMessage = string.Format("^DAW에서 '{0}'이 열려있다면, 지금 반드시 닫아주세요.^\n계속 진행하기 전에 프로젝트를 닫지 않으면 정상적으로 불러오기가 안 될 수도 있습니다.", ellipsizedProjectName);
            alert = new frmAlertPopup(popupSize, Resources.ico_error_b, alertTitle, alertMessage, new OKCancelButtonSet(), null);
            alert.showCloseButton = true;
            alert.showClingLogo = false;
            result = alert.ShowDialog(this);

            if (result == DialogResult.Cancel)
            {
                return;
            }

            if (context.isChanged())
            {
                alertTitle = "아직 기록되지 않은 작업이 있습니다.";
                alertMessage = "현재 작업 상황을 기록하지 않고 진행하면 최근 프로젝트 변경사항은 보관되지 않고 사라지게 됩니다.\n이 버전을 불러오기 전에 지금의 작업상황을 기록해두시겠습니까?";
                alert = new frmAlertPopup(popupSize, Resources.ico_error_b, alertTitle, alertMessage, new CheckoutButtonSet(), null);
                alert.showCloseButton = true;
                alert.showClingLogo = false;
                result = alert.ShowDialog(this);

                if (result == DialogResult.Yes) // 기록 남기기.
                {
                    commit();
                    //return;       //커밋작업 끝내고 바로 불러오기 되도록 return 주석처리.
                }
                else if (result == DialogResult.Cancel) // 취소.
                {
                    return;
                }
            }

            string commitTime = cell.commitInfo.commitDate.ToString(TIMESTAMP_RENDER_FORMAT);
            popupSize = new Size(420, 277);
            alertTitle = "기록된 버전을 불러옵니다";
            alertMessage = string.Format("프로젝트 폴더의 모든 파일이 ${0}$에 저장한 상태로 교체됩니다.\n작업 기록을 남기셨다면 언제든지 원하는 시점으로 프로젝트의 파일 상태를 되돌릴 수 있습니다.",commitTime);
            alert = new frmAlertPopup(popupSize, Resources.ico_complete, alertTitle, alertMessage, new OKCancelButtonSet(), null);
            alert.showCloseButton = true;
            alert.showClingLogo = false;
            result = alert.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    WorkTreeMonitoring.instance.stopMonitoring();
                    context.checkoutVersion(cell.commitInfo.hash);
                    refreshAll(true, true);
                }
                catch (IOException)
                {
                    frmAlertPopup.alert("작업 디렉토리 연결에 실패하였습니다.\n파일이 열려 있지 않은지 확인 후 재시도 해 주십시오.");
                }
                catch (Exception ee)
                {
                    frmAlertPopup.alert(ee.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                    WorkTreeMonitoring.instance.startMonitoring();
                }
            }
        }

        private void showFiles(CommitListCell cell) {
            //파일보기
            frmFileListPopup popup = new frmFileListPopup(context, cell.commitInfo);
            //popup.Size = new System.Drawing.Size(1, 1);
            popup.ShowDialog(this);

            //애니메이션 비스므리한거
            //int plusValue = 10;
            //for (int i = 0; i <= 500; i += plusValue)
            //{
            //    popup.Size = new Size(i, i);

            //    if (i == 300) plusValue = 25;
            //    if (i == 400) plusValue = 50;
            //}
        }

        private CommitListCell createCellFromCommit(Commit commit, bool isCurrentVersion) {
            CommitListCell cell = new CommitListCell(commit, isReadOnly);
            cell.TabStop = false;
            cell.isCurrentVersion = isCurrentVersion;

            return cell;
        }

        private void subCommitList_Load(object sender, EventArgs e) {
            refreshAll(true, true);

            if (context != null) name.Text = context.repoInfo.name;
            name.nameLabel.TextChanged += new EventHandler(onProjectNameChange);
            
            this.MouseWheel += new MouseEventHandler(subCommitList_MouseWheel);

            tableView.Select();
        }

        void subCommitList_MouseWheel(object sender, MouseEventArgs e)
        {
            focusTableView();
        }

        void onProjectNameChange(object sender, EventArgs e)
        {
            context.repoInfo.name = name.Text;
            ClingRayEngine.instance.renameRepository(context.repoInfo.uuid, name.Text);
        }

        private void commitButton_Click(object sender, EventArgs e) {
            commit();
        }

        public void commit() {
            SharedPopupController.UIUpdateDelegate onSuccessfulCommit = result => {
                if (result == SharedPopupController.Result.Success) {
                    refreshAll();
                }
            };

            SharedPopupController.showCommitDialog(context, this, onSuccessfulCommit);
        }

        private void showMoreButton_Click(object sender, EventArgs e) {
            showMore();
        }

        public void goBack() {
            ((frmFrame)this.Parent.Parent).tryShowProjectList();
        }

        private void subCommitList_Click(object sender, EventArgs e)
        {
            tableView.Select();
        }

        private void focusTableView()
        {
            tableView.Select();
            tableView.Focus();
        }

        public void goRefresh()
        {
            refreshAll();
        }
    }
}
