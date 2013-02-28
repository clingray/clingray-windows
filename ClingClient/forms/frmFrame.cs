using System;
using System.Drawing;
using System.Windows.Forms;
using ClingClient.controls;
using System.IO;
using ClingClientEngine;
using ClingClient.forms;
using System.Diagnostics;
using ClingClient.subForms;
using ClingClient.Properties;
using System.Drawing.Imaging;
using ClingClient.common;
using ClingClient.utilities;

namespace ClingClient {
    public partial class frmFrame : Form {
        Bitmap paintBuffer;

        ClingButton btnClose;
        ClingButton btnMin;
        ClingButton btnLogoCling;
        ClingControlBase contentView;
        ClingButton btnPrev = null;
        ClingButton btnRefresh = null;
        ClingImage imgSeperator2 = null;

        public ClingRepo startupRepo { get; set; }
        public bool isStartHidden { get; set; }

        const int RESIZABLE_OFFSET = 30;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        } 

        public frmFrame() {

            InitializeComponent();

            Program.mainframe = this;

            this.Text = "Clingray";
            this.FormBorderStyle = FormBorderStyle.None;
            this.MinimumSize = new Size(727, 590);

            //더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.Load += new System.EventHandler(this.frmFrame_Load);
            this.FormClosed += new FormClosedEventHandler(this.frmFrame_FormClosed);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmFrame_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmFrame_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmFrame_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmFrame_MouseUp);
            this.Resize += new EventHandler(frmFrame_Resize);

            registerBalloonTipHandler();
        }

        private void frmFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (trayIcon.Visible && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                hideWindow(true);
            }
        }

        void frmFrame_FormClosed(object sender, FormClosedEventArgs e)
        {
            stopRepositoryMonitoring();
        }

        void frmFrame_Resize(object sender, EventArgs e) {
            if (WindowState != FormWindowState.Minimized)
            {
                resizableArea = new Rectangle(this.Width - RESIZABLE_OFFSET, this.Height - RESIZABLE_OFFSET, RESIZABLE_OFFSET, RESIZABLE_OFFSET);

                if (paintBuffer != null) paintBuffer.Dispose();
                paintBuffer = makeBackgroundImage(this.Width, this.Height);

                this.Invalidate();
            }
        }

        Bitmap makeBackgroundImage(int formWidth, int formHeight) {
            
            Bitmap bgTotal = new Bitmap(formWidth, formHeight);
            Graphics g = Graphics.FromImage(bgTotal);

            Color contentsBgColor = currentSubForm is subProjectList ? Color.FromArgb(44, 44, 47) : Color.FromArgb(59, 59, 62);

            //top
            Rectangle rectTop = new Rectangle(0, 0, formWidth, 83);
            g.FillRectangle(new SolidBrush(Color.FromArgb(241, 241, 241)), rectTop);


            //로고 아랫쪽 가로선
            int top = 25;
            g.DrawLine(new Pen(Color.FromArgb(218, 218, 218), 1F), 0, top, formWidth, top);
            g.DrawLine(new Pen(Color.FromArgb(250, 250, 250), 1F), 0, top + 1, formWidth, top + 1);


            //contents
            Rectangle rectContents = new Rectangle(0, rectTop.Height, formWidth, formHeight - rectTop.Height);
            g.FillRectangle(new SolidBrush(contentsBgColor), rectContents);


            //전체 테두리
            Rectangle rectAll = new Rectangle(0, 0, formWidth - 1, formHeight - 1);
            g.DrawRectangle(Pens.Black, rectAll);


            return bgTotal;
        }

        private void frmFrame_Paint(object sender, PaintEventArgs e) {
            if (paintBuffer != null) {
                e.Graphics.DrawImage(paintBuffer, 0, 0);
            }
        }

        Point mouseDownPoint = Point.Empty;
        Point preResizePoint = Point.Empty;
        Rectangle resizableArea;

        private void frmFrame_MouseDown(object sender, MouseEventArgs e) {
            mouseDownPoint = e.Location;


            //일단 resize 안되게 고정!!!
            //크기 조정가능 영역에서 마우스 다운
            //if (resizableArea.Contains(e.Location))
            //{
            //    preResizePoint = e.Location;
            //}
        }

        double getGapOffset(int gapX, int gapY) {
            return Math.Sqrt(Math.Pow(gapX, 2) + Math.Pow(gapY, 2));
        }

        private void frmFrame_MouseMove(object sender, MouseEventArgs e) {
            if (!mouseDownPoint.IsEmpty) {
                Point newPoint = e.Location;

                if (!preResizePoint.IsEmpty) //폼 크기조정
                {
                    int gapX = (newPoint.X - preResizePoint.X);
                    int gapY = (newPoint.Y - preResizePoint.Y);

                    double offset = getGapOffset(gapX, gapY);

                    if (offset > 50)  //감도
                    {
                        this.Size = new Size(this.Width + gapX, this.Height + gapY);
                        preResizePoint = newPoint;

                        Console.WriteLine("resized!!! " + DateTime.UtcNow);
                    }
                } else //폼 위치이동
                {
                    int gapX = (newPoint.X - mouseDownPoint.X);
                    int gapY = (newPoint.Y - mouseDownPoint.Y);

                    this.Left += gapX;
                    this.Top += gapY;
                }
            } else {
                //오른쪽 아래 모서리쪽에 마우스가 왔으면,,
                //크기 조정가능 표시 마우스 아이콘
                if (preResizePoint.IsEmpty && resizableArea.Contains(e.Location)) {
                    //일단 resize 안되게 고정!!!

                    //Console.WriteLine("resizable area!!!");
                    //Cursor.Current = Cursors.SizeNWSE;
                }
            }
        }

        private void frmFrame_MouseUp(object sender, MouseEventArgs e) {
            mouseDownPoint = Point.Empty;
            preResizePoint = Point.Empty;
        }

        private void frmFrame_Load(object sender, EventArgs e) {
            setControls();

            if (this.startupRepo == null)
            {
                tryShowProjectList();
            }
            else
            {
                showCommitList(this.startupRepo);
            }
        }

        public ContainerControl getDataView() {
            return contentView;
        }

        void setControls() {
            bool applyAnchor = false;

            //닫기 버튼
            btnClose = new ClingButton();
            btnClose.setImages(Properties.Resources.btn_apptop_close02_normal,
                Properties.Resources.btn_apptop_close02_press,
                Properties.Resources.btn_apptop_close02_hover);
            btnClose.moveParentTopRight(this, 5, 3);
            btnClose.ButtonClick += new EventHandler(btnClose_ButtonClick);
            if (applyAnchor) btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            //최소화 버튼
            btnMin = new ClingButton();
            btnMin.setImages(Properties.Resources.btn_apptop_min_normal,
                Properties.Resources.btn_apptop_min_press,
                Properties.Resources.btn_apptop_min_hover);
            btnMin.moveLeft(btnClose, 3);
            btnMin.ButtonClick += new EventHandler(btnMin_ButtonClick);
            if (applyAnchor) btnMin.Anchor = AnchorStyles.Right | AnchorStyles.Top;



            //이전화면 버튼
            btnPrev = new ClingButton();
            btnPrev.setImages(Properties.Resources.btn_prev_normal,
                Properties.Resources.btn_prev_press,
                Properties.Resources.btn_prev_hover);
            btnPrev.ButtonClick += new EventHandler(btnPrev_ButtonClick);
            btnPrev.setToolTip("이전화면으로 돌아가기");

            //새로고침과 이전화면 사이 seperator
            imgSeperator2 = new ClingImage(Resources.div_apptop);

            //새로고침 버튼
            btnRefresh = new ClingButton();
            btnRefresh.setImages(Properties.Resources.btn_ref_normal,
                Properties.Resources.btn_ref_press,
                Properties.Resources.btn_ref_hover);
            btnRefresh.ButtonClick += new EventHandler(btnRefresh_ButtonClick);
            btnRefresh.setToolTip("새로고침");



            

            //메인메뉴 버튼 (로고)
            btnLogoCling = new ClingButton();
            btnLogoCling.bmpNormal = new Bitmap(Properties.Resources.btn_logo_cling);
            btnLogoCling.Location = new Point(10, 6);
            btnLogoCling.ButtonClick += new EventHandler(btnLogoCling_ButtonClick);

            



            //메인 컨텐츠 부분
            contentView = new ClingControlBase();
            contentView.Location = new Point(0, 59 + 24);
            contentView.Size = new Size(this.Width, this.Height - contentView.Top - 18);
            if (applyAnchor) contentView.Anchor = AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Top;
            //contentView.BackColor = Color.Red;



            this.Controls.Add(btnClose);
            this.Controls.Add(btnMin);
            this.Controls.Add(btnLogoCling);
            this.Controls.Add(contentView);
        }

        void btnRefresh_ButtonClick(object sender, EventArgs e)
        {
            if (currentSubForm != null)
            {
                currentSubForm.goRefresh();
            }
        }

        void btnLogoCling_ButtonClick(object sender, EventArgs e) {
            ContextMenu m = new ContextMenu();

            MenuItem itemConfig = new MenuItem("환경설정");
            itemConfig.Click += new EventHandler(itemConfig_Click);

            MenuItem itemAlwaysTop = new MenuItem("항상 위에 두기");
            itemAlwaysTop.Checked = isAlwaysOnTop;
            itemAlwaysTop.Click += new EventHandler(itemAlwaysTop_Click);

            MenuItem itemAddProject = new MenuItem("프로젝트 추가");
            itemAddProject.Click += new EventHandler(itemAddProject_Click);

            MenuItem itemHelp = new MenuItem("도움말/문의하기");
            itemHelp.Click += new EventHandler(itemHelp_Click);

            MenuItem itemCheckUpdate = new MenuItem("업데이트 확인");
            itemCheckUpdate.Click += new EventHandler(itemCheckUpdate_Click);

            MenuItem itemAbout = new MenuItem("프로그램 정보");
            itemAbout.Click += new EventHandler(itemAbout_Click);

            MenuItem itemGoWebsite = new MenuItem("웹사이트 가기");
            itemGoWebsite.Click += new EventHandler(itemGoWebsite_Click);

            MenuItem itemQuit = new MenuItem("종료");
            itemQuit.Click += new EventHandler(itemQuit_Click);

            m.MenuItems.Add(itemConfig);
            m.MenuItems.Add(itemAlwaysTop);
            m.MenuItems.Add("-");
            m.MenuItems.Add(itemAddProject);
            m.MenuItems.Add("-");
            m.MenuItems.Add(itemHelp);
            m.MenuItems.Add(itemCheckUpdate);
            m.MenuItems.Add(itemAbout);
            m.MenuItems.Add(itemGoWebsite);            
            m.MenuItems.Add("-");
            m.MenuItems.Add(itemQuit);

            m.Show(this, new Point(40, 25));
        }

        void itemQuit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        
        void itemAbout_Click(object sender, EventArgs e) {
            frmAboutPopup popup = new frmAboutPopup(frmAboutPopup.DisplayMode.ABOUT);
            popup.ShowDialog(this);
        }

        void itemCheckUpdate_Click(object sender, EventArgs e)
        {
            checkUpdate();
        }

        void itemGoWebsite_Click(object sender, EventArgs e) {
            Process.Start("http://www.clingray.com/account/profile");
        }

        void itemHelp_Click(object sender, EventArgs e) {
            Process.Start("http://clingray.desk.com");
        }

        //프로젝트 추가
        public void itemAddProject_Click(object sender, EventArgs e) {
            frmAddProjectPopup popup = new frmAddProjectPopup();
            if (popup.ShowDialog(this) == DialogResult.OK)
            {
                if (frmAddProjectPopup.exitString == null)  //프로젝트 추가 성공
                {
                    showProjectList();
                }
                else //이미 있는 프로젝트 선택해서, 커밋 리스트 보기로 전환
                {
                    RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(frmAddProjectPopup.exitString);
                    showCommitList(context.repoInfo);
                }
            }
        }

        //항상위에
        bool isAlwaysOnTop = false;
        void itemAlwaysTop_Click(object sender, EventArgs e) {
            MenuItem itemAlwaysTop = (MenuItem)sender;
            isAlwaysOnTop = !isAlwaysOnTop;

            this.TopMost = isAlwaysOnTop;
        }

        //환경설정
        void itemConfig_Click(object sender, EventArgs e) {
            launchConfig();
        }

        //이전
        void btnPrev_ButtonClick(object sender, EventArgs e) {
            if (currentSubForm != null) {
                currentSubForm.goBack();
            }
        }

        void btnMin_ButtonClick(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
        }

        void btnClose_ButtonClick(object sender, EventArgs e) {
            this.Close();
        }

        void removeControl(Control c)
        {
            if (this.Controls.Contains(c)) this.Controls.Remove(c);
        }

        //이전으로가기, 새로고침 버튼 설정
        void setPrevRefreshButton(ClingControlBase subForm)
        {
            removeControl(btnPrev);
            removeControl(imgSeperator2);
            removeControl(btnRefresh);

            if (subForm is subCommitList)
            {
                btnPrev.Location = new Point(15, 40);
                imgSeperator2.moveRight(btnPrev, 14);
                btnRefresh.moveRight(imgSeperator2, 14);

                this.Controls.Add(btnPrev);
                this.Controls.Add(imgSeperator2);
                this.Controls.Add(btnRefresh);
            }
            else
            {
                btnRefresh.Location = new Point(15, 40);

                this.Controls.Add(btnRefresh);
            }
        }

        subFormInterface currentSubForm = null;
        void changeDataContent(ClingControlBase subForm)
        {
            subFormInterface oldSubFormObj = currentSubForm;
            getDataView().Controls.Remove(oldSubFormObj as Control);
            if (oldSubFormObj != null)
            {
                Control f = oldSubFormObj as Control;

                //툴팁 memory leck 방지를 위한 수동 dispose 
                ToolTipHelper.removeFromControl(f); 

                f.Dispose();
                f = null;
            }
            
            currentSubForm = (subFormInterface)subForm;

            //이전으로 돌아가기 화살표 표시 여부 설정
            setPrevRefreshButton(subForm);

            subForm.Location = new Point(0, 0);
            subForm.Size = getDataView().Size;

            getDataView().Controls.Add(subForm);

            if (paintBuffer != null) paintBuffer.Dispose();
            paintBuffer = makeBackgroundImage(this.Width, this.Height);

            System.GC.Collect();
            this.Invalidate();
        }

        //cling 데이터 디렉토리 지정 action
        public void setBackupDir() {
            setMainDimmed(true);
            changeDataContent(new subBackupDirSelect());
        }

        public bool prepareBackupDir(string dotGitPath)
        {
            //상대경로로 적었으면 안되게!
            if (!Path.IsPathRooted(dotGitPath))
            {
                frmAlertPopup.alert(dotGitPath + "\n잘못된 백업 공간 경로 입니다!");
                return false;
            }

            //루트디렉토리는 설정하지 못하도록 막는다!
            if (!ClingRayEngine.instance.canSetBackupDirectory(dotGitPath))
            {
                frmAlertPopup.alert(dotGitPath + "\n백업 공간으로 사용할 수 없습니다.\n하나의 폴더를 지정해주세요.");
                return false;
            }

            if (!Directory.Exists(dotGitPath))
            {
                try
                {
                    //디렉토리 생성
                    Directory.CreateDirectory(dotGitPath);

                }
                catch (Exception ee)
                {
                    frmAlertPopup.alert(ee.Message);
                    return false;
                }
            }

            return true;
        }

        //프로젝트 리스트 action
        public void showProjectList(string dotGitPath) 
        {
            EngineConfig.repositoryContainerPath = dotGitPath;
            showProjectList();
        }

        public void tryShowProjectList() {
            if (EngineConfig.isEmptyConfig())
            {
                setBackupDir();
            } else 
            {
                showProjectList();
            }
        }

        public void showProjectList() {
            Cursor.Current = Cursors.WaitCursor;
            trayIcon.Visible = true;
            trayIcon.Text = string.Format("{0} {1}", Application.ProductName, ApplicationUtils.getApplicationVersion());

            setMainDimmed(false);

            try {
                EngineConfig.authorName = Program.commiter_email;
                EngineConfig.authorEmail = Program.commiter_email;

                ClingRayEngine.instance.initialize();

                startRepositoryMonitoring();

                changeDataContent(new subProjectList());

                Cursor.Current = Cursors.Default;
            } catch (Exception ee) {
                Cursor.Current = Cursors.Default;
                frmAlertPopup.alert(ee.Message);
            }

            reconstructTrayContextMenu(false);
        }

        public void showCommitList(ClingRepo repo)
        {
            //showCommitListByTree(repo);
            showCommitListByList(repo);
        }

        public void showCommitListByList(ClingRepo repo) {
            if (!repo.dontHaveDotGit())
            {
                subCommitList f = new subCommitList();
                f.repositoryUUID = repo.uuid;

                RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(repo);

                RepositoryAccessHistoryManager.instance.addHistory(repo.uuid, DateTime.UtcNow);

                if (repo.hasWorkTree())
                {
                    int MAX_REPO_NAME_IN_TOOLTIP = 20;
                    trayIcon.Text = string.Format("{0} {1}\n프로젝트: {2}",
                        Application.ProductName,
                        ApplicationUtils.getApplicationVersion(),
                        StringUtils.getStringWithEllipsis(repo.name, MAX_REPO_NAME_IN_TOOLTIP));

                    reconstructTrayContextMenu(true);
                }

                changeDataContent(f);
            }
        }

        public void showCommitListByTree(ClingRepo repo)
        {
            if (!repo.dontHaveDotGit())
            {
                subCommitTree f = new subCommitTree(repo);
                changeDataContent(f);
            }
        }

        private void frmFrame_Shown(object sender, EventArgs e)
        {
            if (isStartHidden)
            {
                hideWindow(false);
            }
        }

        Bitmap ChangeOpacity(Image img, float opacityvalue)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height); // Determining Width and Height of Source Image
            Graphics graphics = Graphics.FromImage(bmp);
            ColorMatrix colormatrix = new ColorMatrix();
            colormatrix.Matrix33 = opacityvalue;
            ImageAttributes imgAttribute = new ImageAttributes();
            imgAttribute.SetColorMatrix(colormatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
            graphics.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imgAttribute);
            graphics.Dispose();   // Releasing all resource used by graphics 
            return bmp;
        }

        public void setMainDimmed(bool doDim)
        {
            btnLogoCling.bmpNormal = doDim ? ChangeOpacity(Properties.Resources.btn_logo_cling, 0.2f) : Properties.Resources.btn_logo_cling;

            btnRefresh.bmpNormal = doDim ? ChangeOpacity(Properties.Resources.btn_ref_normal, 0.2f) : Properties.Resources.btn_ref_normal;
            btnRefresh.Enabled = !doDim;

            btnLogoCling.Enabled = !doDim;
        }

        private void switchToRepository(string uuid)
        {
            ClingRepo repo = ClingRayEngine.instance.repositories.getRepository(uuid);

            if (repo == null)
            {
                frmAlertPopup.alert("존재하지 않는 프로젝트입니다.", true);
            }
            else
            {
                showCommitList(repo);
            }
        }

        private void launchConfig()
        {
            frmConfigPopup popup = new frmConfigPopup();
            if (popup.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                switch (frmConfigPopup.exitFlag)
                {
                    case 3:     //OK
                        showProjectList(frmConfigPopup.exitString);
                        break;
                }
            }
        }
    }
}
