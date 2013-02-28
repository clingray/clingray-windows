using System;
using System.Drawing;
using System.Windows.Forms;
using ClingClient.forms;
using System.IO;
using ClingClient.common;
using System.Threading;
using ClingClientEngine;
using ClingClient.utilities;

namespace ClingClient.popup
{
    public partial class frmPreScan : frmCommonPopup
    {
        ListView lvProject;
        ClingLabel lblCurDir;

        delegate void PRINT_DIR(string dir);
        PRINT_DIR printDir;

        delegate void PRINT_FILE(string path);
        PRINT_FILE printFile;

        Thread scanThread = null;
        ScanButtonSet buttonSet = null;
        bool _stopScan = false;

        public frmPreScan()
        {
            InitializeComponent();

            buttonSet = new ScanButtonSet(this);
            printDir = new PRINT_DIR(_printDir);
            printFile = new PRINT_FILE(_printFile);

            this.Size = new Size(800, 500);

            this.showButtonLine = true;
            this.showCloseButton = true;
            this.showClingLogo = true;

            setControls();

            
            this.Load += new EventHandler(frmPreScan_Load);
            this.Resize += new EventHandler(frmPreScan_Resize);
            this.FormClosing += new FormClosingEventHandler(frmPreScan_FormClosing);
        }

        void frmPreScan_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopScan();
        }

        void frmPreScan_Resize(object sender, EventArgs e)
        {
            buttonSet.arrangeButtons(this);
        }

        void frmPreScan_Load(object sender, EventArgs e)
        {
            startScan();
        }

        void _printDir(string dir)
        {
            lblCurDir.label.Text = dir;
        }

        void _printFile(string path)
        {
            string dir = Path.GetDirectoryName(path);
            string file = Path.GetFileName(path);

            //디렉토리 중복으로 안되게 처리.. (나중에 확인 필요!!)
            if (!lvProject.Items.ContainsKey(dir))
            {
                ListViewItem item = new ListViewItem(new string[] { dir, file });
                item.ImageKey = dir;
                lvProject.Items.Add(item);
            }
        }

        void setControls()
        {
            int margin = 20;

            lblCurDir = new ClingLabel("-");
            lblCurDir.label.AutoSize = false;
            lblCurDir.label.AutoEllipsis = true;
            lblCurDir.Width = this.Width - margin * 2;
            lblCurDir.Top = margin * 2;
            lblCurDir.Left = margin;
            lblCurDir.label.Width = lblCurDir.Width;


            lvProject = new ListView();
            lvProject.CheckBoxes = true;
            lvProject.FullRowSelect = true;
            lvProject.Size = new System.Drawing.Size(this.Width - margin * 2, this.Height - margin * 3 - lblCurDir.Height - (this.Height-getAboveButtonLineTop()));
            lvProject.Location = new System.Drawing.Point(margin, lblCurDir.Top + lblCurDir.Height);
            
            lvProject.View = View.Details;

            ColumnHeader h1 = new ColumnHeader();
            h1.Text = "경로";
            h1.Width = (int)(lvProject.Width * (2F / 3));
            lvProject.Columns.Add(h1);

            ColumnHeader h2 = new ColumnHeader();
            h2.Text = "프로젝트 파일";
            h2.Width = (int)(lvProject.Width * (0.9F / 3));
            lvProject.Columns.Add(h2);

            this.Controls.Add(lblCurDir);
            this.Controls.Add(lvProject);

            buttonSet.addButtons(this);
        }

        void scanDirectory(string parentPath)
        {
            if (_stopScan) return;

            //Console.WriteLine(parentPath);
            this.Invoke(printDir, new object[] { parentPath });

            try
            {
                foreach (string path in Directory.GetDirectories(parentPath))
                {
                    if (_stopScan) return;

                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    if (
                        ((dirInfo.Attributes & FileAttributes.System) > 0) ||
                        ((dirInfo.Attributes & FileAttributes.Hidden) > 0) ||
                        ((dirInfo.Attributes & FileAttributes.Temporary) > 0)
                    ) continue;

                    scanDirectory(path);
                }
            }
            catch
            {
            }

            

            try
            {
                foreach (string pattern in WorkTreeMonitoring.DAW_PROJECT_FILE_FILTERS)
                {
                    foreach (string path in Directory.GetFiles(parentPath, pattern))
                    {
                        if (_stopScan) return;

                        //Console.WriteLine(path);
                        this.Invoke(printFile, new object[] { path });
                    }
                }
            }
            catch
            {
            }
        }

        void startScan()
        {
            lvProject.Items.Clear();

            scanThread = new Thread(new ThreadStart(_startScan));
            scanThread.Start();
        }

        void _startScan()
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady) continue;

                //Console.WriteLine(drive);
                scanDirectory(drive.RootDirectory.FullName);
            }

            Console.WriteLine("scan thread end");
        }

        void stopScan()
        {
            _stopScan = true;
            Application.DoEvents();

            scanThread.Join();
        }

        public void startRegister()
        {
            if (lvProject.CheckedItems.Count == 0)
            {
                frmAlertPopup.alert("자동으로 등록하실 관리 폴더를 선택해 주세요.");
                return;
            }

            frmAlertPopup popup = new frmAlertPopup(
                    new Size(300, 220),
                    Properties.Resources.ico_error_b,
                    "선택하신 " + lvProject.CheckedItems.Count + " 개의 폴더를\n관리 프로젝트로 자동 등록하시겠습니까?",
                    null,
                    new OKCancelButtonSet(), null
                );
            popup.StartPosition = FormStartPosition.CenterParent;
            if (popup.ShowDialog(this) == DialogResult.OK)
            {
                stopScan();

                foreach (ListViewItem item in lvProject.CheckedItems)
                {
                    string workingDirPath = item.SubItems[0].Text;
                    string workingDirName = Path.GetFileName(workingDirPath);

                    Console.WriteLine(workingDirPath + "\t" + workingDirName);

                    RepositoryContext currentRepoContext = ClingRayEngine.instance.addRepository(workingDirName, workingDirPath, RepoHelper.makeCoverImageIndex());
                }

                this.Close();
            }
        }
    }


    public class ScanButtonSet : IAlertButtonSet
    {
        ClingButton btnRegister;
        ClingButton btnClose;
        ClingButton btnStop;
        frmPreScan parentForm;

        public ScanButtonSet(frmPreScan parentForm)
        {
            this.parentForm = parentForm;   
        }

        public void addButtons(frmCommonPopup popup)
        {
            btnClose = new ClingButton();
            btnClose.setImages(Properties.Resources.btn_popup_close_normal,
                Properties.Resources.btn_popup_close_press,
                Properties.Resources.btn_popup_close_normal);
            btnClose.ButtonClick += new EventHandler(btnClose_ButtonClick);

            btnRegister = new ClingButton();
            btnRegister.setImages(Properties.Resources.btn_popup_enter_normal,
                Properties.Resources.btn_popup_enter_press,
                Properties.Resources.btn_popup_enter_normal);
            btnRegister.ButtonClick += new EventHandler(btnRegister_ButtonClick);

            popup.Controls.Add(btnClose);
            popup.Controls.Add(btnRegister);
        }

        void btnClose_ButtonClick(object sender, EventArgs e)
        {
            parentForm.Close();
        }

        void btnRegister_ButtonClick(object sender, EventArgs e)
        {
            parentForm.startRegister();
        }

        public void arrangeButtons(frmCommonPopup popup)
        {
            btnClose.moveParentBottomRight(popup, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);
            btnRegister.moveLeft(btnClose, 5);
        }

    }
}
