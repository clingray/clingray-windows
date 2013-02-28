using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using ClingClientEngine;
using ClingClient.forms;
using System.Diagnostics;
using ClingClient.popup;
using System.Drawing.Drawing2D;
using ClingClient.utilities;
using ClingClient.common;
using ClingClient.Properties;
using ClingClient.controls;
using System.IO;

namespace ClingClient
{
    public partial class ClingProjectInfo : ClingControlBase
    {
        Bitmap bmpCDOut = Properties.Resources.btn_disk_hover;
        Bitmap bmpCDIn = Properties.Resources.btn_disk_normal;
        ClingButton btnLinkStatus;
        ClingButton btnSetting;

        Bitmap _bmpCDJacket;
        public Bitmap bmpCDJacket
        {
            get
            {
                return _bmpCDJacket;
            }
            set
            {
                _bmpCDJacket = value;
                makeBackgroundImage(bmpCDIn);
            }
        }

        public event EventHandler onProjectInfoClick;
        public event EventHandler onProjectListShouldReLoad;

        public ClingRepo repo = null;
        Rectangle recCDJacket = new Rectangle(0, 0, 200, 152);

        static Font prjNameFont = null;
        static Font prjVersionCntFont = null;

        static ClingProjectInfo()
        {
            prjNameFont = new Font("맑은 고딕", 12, FontStyle.Bold);
            prjVersionCntFont = new Font("Arial", 9);
        }

        public ClingProjectInfo(ClingRepo repo)
        {
            InitializeComponent();

            this.repo = repo;
            this.Size = new System.Drawing.Size(200, 182);

            if (repo == null)  //프로젝트 추가
            {
                bmpCDOut = Properties.Resources.btn_disk_add_hover;
                bmpCDIn = Properties.Resources.btn_disk_add_normal;

                this.MouseEnter += new EventHandler(ClingProjectInfo_MouseEnter);
                this.MouseLeave += new EventHandler(ClingProjectInfo_MouseLeave);
            }
            else
            {
                
                //연결상태
                btnLinkStatus = new ClingButton();
                setLinkButtonStatus();
                btnLinkStatus.moveParentBottomLeft(this, 108, 0);
                
                this.Controls.Add(btnLinkStatus);
                

                bmpCDOut = Properties.Resources.btn_disk_hover;
                bmpCDIn = Properties.Resources.btn_disk_normal;


                //관리중인 정상 repo인 경우에만 CD튀어나오는 액션 일어나도록
                if (repo.hasWorkTree())
                {
                    this.MouseEnter += new EventHandler(ClingProjectInfo_MouseEnter);
                    this.MouseLeave += new EventHandler(ClingProjectInfo_MouseLeave);
                }


                //설정버튼
                btnSetting = new ClingButton();
                btnSetting.setImages(Properties.Resources.btn_setting_normal,
                    Properties.Resources.btn_setting_press,
                    Properties.Resources.btn_setting_normal);
                btnSetting.moveParentTopRight(this, 7, 4);
                btnSetting.ButtonClick += new EventHandler(btnSetting_ButtonClick);

                this.Controls.Add(btnSetting);
            }

            this.Click += new EventHandler(ClingProjectInfo_Click);
            
        }

        public void setLinkButtonStatus()
        {
            if (repo.dontHaveDotGit())
            {
                //.git 없음
                btnLinkStatus.setImages(Properties.Resources.btn_state_record_disable, null, null);
                btnLinkStatus.setToolTip("빈 프로젝트");
                btnLinkStatus.ButtonClick -= new EventHandler(btnLinkStatus_ButtonClick);
            }
            else
            {
                if (!repo.hasWorkTree())
                {
                    //worktree 없음
                    btnLinkStatus.setImages(Properties.Resources.btn_state_record_off, null, null);
                    btnLinkStatus.setToolTip("프로젝트 기록 복구/재시작");
                    btnLinkStatus.ButtonClick += new EventHandler(btnLinkStatus_ButtonClick);
                }
                else
                {
                    //정상
                    btnLinkStatus.setImages(Properties.Resources.btn_state_record_on, null, null);
                    btnLinkStatus.setToolTip("작업 기록 중인 프로젝트 입니다.");
                    btnLinkStatus.ButtonClick -= new EventHandler(btnLinkStatus_ButtonClick);
                }
            }
        }

        //.git과 다시 연결!
        void btnLinkStatus_ButtonClick(object sender, EventArgs e)
        {
            bool shouldDeleteWorkTree;
            string workTreePath = SharedPopupController.showSelectNewWorkTree(this, out shouldDeleteWorkTree);

            if (workTreePath!=null)
            {
                if (ClingRayEngine.instance.linkWorkTree(repo.uuid, workTreePath))
                {
                    try
                    {
                        if (shouldDeleteWorkTree)
                        {
                            Cursor.Current = Cursors.WaitCursor;

                            RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(workTreePath);
                            Commit commit = context.getCurrentCommit();
                            if (context.checkoutVersion(commit.hash))
                            {
                                Cursor.Current = Cursors.Default;
                                ClingProjectInfo_Click(this, e);
                            }

                            Cursor.Current = Cursors.Default;
                        }
                        else
                        {
                            ClingProjectInfo_Click(this, e);
                        }
                    }
                    catch (IOException)
                    {
                        ClingRayEngine.instance.unlinkWorkTree(workTreePath);
                        Cursor.Current = Cursors.Default;

                        frmAlertPopup.alert("작업 디렉토리 연결에 실패하였습니다.\n파일이 열려 있지 않은지 확인 후 재시도 해 주십시오.");
                    }
                    catch (Exception)
                    {
                        ClingRayEngine.instance.unlinkWorkTree(workTreePath);
                        Cursor.Current = Cursors.Default;

                        frmAlertPopup.alert("작업 디렉토리 연결에 실패하였습니다.");
                    }
                }
                else
                {
                    frmAlertPopup.alert("작업 디렉토리 연결에 실패하였습니다.");
                }
            }
        }

        void btnSetting_ButtonClick(object sender, EventArgs e)
        {
            ContextMenu m = new ContextMenu();

            MenuItem itemUnlink = new MenuItem("작업기록 중단");
            itemUnlink.Click += new EventHandler(itemUnlink_Click);

            MenuItem itemClearHistory = new MenuItem("작업기록 모두 삭제");
            if (repo.dontHaveDotGit())     //로컬repo없는 경우 작업기록삭제 메뉴 비활성화!
            {
                itemClearHistory.Enabled = false;
            }
            else
            {
                itemClearHistory.Click += new EventHandler(itemClearHistory_Click);
            }


            

            MenuItem itemOpenExploer = new MenuItem("탐색기로 프로젝트 폴더 열기");
            itemOpenExploer.Click += new EventHandler(itemOpenExploer_Click);

            


            if (repo.workTrees.Count == 0)
            {
                //관리중이지 않은 프로젝트에 대해서는 기록중단, 탐색기 열기 disable (worktree가 없어서 불가능!)

                itemUnlink.Enabled = false;      
                itemOpenExploer.Enabled = false;
            }

            m.MenuItems.Add(itemUnlink);
            m.MenuItems.Add(itemClearHistory);
            m.MenuItems.Add("-");
            m.MenuItems.Add(itemOpenExploer);

            m.Show(this, new Point(this.Width-30, 25));
        }

        void itemOpenExploer_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", repo.workTrees[0].path);
        }

        //작업기록 삭제
        void itemClearHistory_Click(object sender, EventArgs e)
        {
            frmAlertPopup popup = new frmAlertPopup(
                new Size(398, 265),
                Properties.Resources.ico_warning,
                "작업 기록을 삭제하시겠습니까?",
                "이 프로젝트의 작업 기록을 더 이상 할 수 없게 됩니다.\n현재 작업중인 프로젝트의 파일은 변경되지 않으나 $내 컴퓨터에 백업된 파일과 기록이 모두 삭제$됩니다.",
                new OKCancelButtonSet(), null
            );

            if (popup.ShowDialog(this) == DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;

                try
                {
                    ClingRayEngine.instance.removeRepository(repo.uuid);
                    WorkTreeMonitoring.instance.stopMonitoringByRepoId(repo.uuid);
                    if (onProjectListShouldReLoad != null) onProjectListShouldReLoad(sender, e);

                    popup = new frmAlertPopup(
                        new Size(398, 263),
                        Properties.Resources.ico_warning,
                        "작업 기록이 삭제되었습니다.",
                        "내 컴퓨터의 백업 파일이 삭제되었습니다.\n작업 기록을 다시 시작하시려면 목록에서 $'프로젝트 추가'$로 현시점부터 다시 기록을 시작하시면 됩니다.",
                        new CloseButtonSet(), null
                    );

                    popup.ShowDialog(this);
                }
                catch (Exception ee)
                {
                    frmAlertPopup.alert(ee.Message);
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
            else
            {
            }
        }

        //작업기록 중단
        void itemUnlink_Click(object sender, EventArgs e)
        {
            SharedPopupController.UIUpdateDelegate onSuccessfulUnlink = result => {
                if (result == SharedPopupController.Result.Success && onProjectListShouldReLoad != null) {
                    onProjectListShouldReLoad("STOP", e);
                }
            };

            DialogResult answer = SharedPopupController.showUnlinkWorkTreeDialog(repo.workTrees[0].path, this, onSuccessfulUnlink);
        }

        void ClingProjectInfo_Click(object sender, EventArgs e)
        {
            if (onProjectInfoClick != null)
            {
                onProjectInfoClick(sender, e);
            }
        }

        void makeBackgroundImage(Bitmap bgImage)
        {
            Graphics g = getDC();
            g.SmoothingMode = SmoothingMode.AntiAlias;

            //배경 공통 CD이미지
            g.DrawImage(bgImage, 0, 0);

            if(repo != null)
            {
                //CD자켓 이미지 투명 처리
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = 1.0f; //opacity 0 = completely transparent, 1 = completely opaque
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                //CD자켓 이미지 출력
                if(bmpCDJacket!=null) g.DrawImage(bmpCDJacket, recCDJacket, 0, 0, bmpCDJacket.Width, bmpCDJacket.Height, GraphicsUnit.Pixel, attributes);
                
                //프로젝트명
                string repoName = StringUtils.getStringWithEllipsis(repo.name, 30);

                Rectangle CDBoxRec = new Rectangle(54, 27, 140, 90);
                StringFormat RepoStringFormat = new StringFormat();
                RepoStringFormat.Alignment = StringAlignment.Center;
                RepoStringFormat.LineAlignment = StringAlignment.Center;

                SizeF textSize = g.MeasureString(repoName, prjNameFont, CDBoxRec.Width, RepoStringFormat);

                float x = CDBoxRec.Left;
                float y = CDBoxRec.Top;

                Brush shadowBrush = new SolidBrush(Color.FromArgb(0x0d, 0x0d, 0x0d));
                Rectangle CDBoxRecShadow = new Rectangle(CDBoxRec.Location, CDBoxRec.Size);
                CDBoxRecShadow.Offset(0, -2);

                g.DrawString(repoName, prjNameFont, shadowBrush, CDBoxRecShadow, RepoStringFormat);
                g.DrawString(repoName, prjNameFont, Brushes.White, CDBoxRec, RepoStringFormat);

                if (!repo.dontHaveDotGit())
                {
                    //버젼수
                    string versionCount = string.Empty;

                    try
                    {
                        RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(repo);
                        versionCount = Convert.ToString(context.getVersionCount());
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("비정상 프로젝트: ", repo.name);
                    }

                    if (!string.IsNullOrWhiteSpace(versionCount))
                    {
                        textSize = g.MeasureString(versionCount, prjVersionCntFont);

                        x = recCDJacket.Right - textSize.Width - 16;
                        y = recCDJacket.Bottom - textSize.Height - 16;

                        shadowBrush = new SolidBrush(Color.FromArgb(0x20, 0x1f, 0x1c));
                        g.DrawString(versionCount, prjVersionCntFont, Brushes.White, x, y - 1);
                        g.DrawString(versionCount, prjVersionCntFont, Brushes.White, x, y);
                    }                
                }
            }
        }

        void ClingProjectInfo_MouseLeave(object sender, EventArgs e)
        {
            makeBackgroundImage(bmpCDIn);
            this.Invalidate();
        }

        void ClingProjectInfo_MouseEnter(object sender, EventArgs e)
        {
            makeBackgroundImage(bmpCDOut);
            this.Invalidate();
        }

        //관리중인 디렉토리에 파일 변경있을때
        internal void setModifiedStatus()
        {
            ClingImage Indicator = new ClingImage(Resources.ico_disk_update);
            Indicator.moveParentTopLeft(54, 2);

            this.Controls.Add(Indicator);
        }
    }
}
