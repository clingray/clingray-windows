using System;
using System.Drawing;
using System.Windows.Forms;
using ClingClient.controls;
using ClingClient.subForms;
using System.IO;
using ClingClientEngine;
using ClingClient.Properties;

namespace ClingClient.forms
{
    public partial class subBackupDirSelect : ClingControlBase, subFormInterface
    {
        ClingTextBox txtPath;
        ClingButton btnSelectPath;
        ClingLabel lblCmt1;
        ClingLabel lblCmt2;
        ClingLabel lblCmt3;
        ClingButton btnStart;

        const string default_clingray_backup_dir_name = "ClingrayBackups";

        public subBackupDirSelect()
        {
            InitializeComponent();
            setControls();
            this.Resize += new EventHandler(subBackupDirSelect_Resize);
        }

        /* 디폴트 백업경로 생성
         * 
         * D:\Clingray 를 기본으로 하되,, 해당 드라이브가 존재하지 않으면 user폴더아래로 제시!
         */
        string getDefaultDir()
        {
            string dirPath = null;

            try
            {
                //D드라이브가 존재하는지 검사
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    if (!drive.IsReady || (drive.AvailableFreeSpace<1)) continue;       //준비된 드라이브만! (cdrom같은놈 무시되게!)
                    if (drive.AvailableFreeSpace < 1024 * 1024 * 1024 /* 1GB */) continue;
                    if (drive.Name.Equals(@"C:\")) continue;

                    dirPath = Path.Combine(drive.Name, default_clingray_backup_dir_name);
                    break;
                }
            }
            catch
            {
            }


            //D드라이이브 존재하지 않으면 user폴더로!
            if (dirPath == null)
            {
                dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), default_clingray_backup_dir_name);
            }
           
            return dirPath;
        }

        void setControls()
        {
            txtPath = new ClingTextBox("백업 경로를 지정해 주세요.");
            txtPath.Size = new System.Drawing.Size(240, 25);
            txtPath.setText(getDefaultDir());

            btnSelectPath = new ClingButton();
            btnSelectPath.setImages(Properties.Resources.btn_backup_change_normal,
                Properties.Resources.btn_backup_change_press,
                Properties.Resources.btn_backup_change_normal);
            btnSelectPath.ButtonClick += new EventHandler(btnSelectPath_ButtonClick);


            lblCmt1 = new ClingLabel("지정한 폴더에 해당 프로젝트의 모든 파일을 자동으로 백업하게 됩니다.");
            lblCmt1.label.ForeColor = Color.FromArgb(0x737478);
            lblCmt1.label.Font = new System.Drawing.Font("돋움", 12F, GraphicsUnit.Pixel);

            lblCmt2 = new ClingLabel("작업용 디스크보다는, 백업용 공간에 폴더를 만들어주세요.");
            lblCmt2.label.ForeColor = Color.FromArgb(0x737478);
            lblCmt2.label.Font = new System.Drawing.Font("돋움", 12F, GraphicsUnit.Pixel);

            lblCmt3 = new ClingLabel("권장용량 : 2G 이상");
            lblCmt3.label.ForeColor = Color.FromArgb(0x15AEE4);
            lblCmt3.label.Font = new System.Drawing.Font("돋움", 12F, GraphicsUnit.Pixel);

            btnStart = new ClingButton();
            btnStart.setImages(Properties.Resources.btn_cling_start_normal,
                Properties.Resources.btn_cling_start_press,
                Properties.Resources.btn_cling_start_normal);
            btnStart.ButtonClick += new EventHandler(btnStart_ButtonClick);


            this.Controls.Add(txtPath);
            this.Controls.Add(btnSelectPath);
            this.Controls.Add(lblCmt1);
            this.Controls.Add(lblCmt2);
            this.Controls.Add(lblCmt3);
            this.Controls.Add(btnStart);
        }

        void btnStart_ButtonClick(object sender, EventArgs e)
        {
            if (!Program.mainframe.prepareBackupDir(txtPath.getText())) return;

            EngineConfig.repositoryContainerPath = txtPath.getText();
            Program.mainframe.tryShowProjectList();
        }

        void btnSelectPath_ButtonClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();

            String selectedDir = dlg.SelectedPath;
            if ((selectedDir != null) && (selectedDir.Length > 0)) txtPath.setText(selectedDir);
        }

        void subBackupDirSelect_Resize(object sender, EventArgs e)
        {
            makeBgImage();
            this.Invalidate();

            //control들 위치 재배치!
            float totalPathWidth = txtPath.Width + btnSelectPath.Width + 5;
            txtPath.Location = new Point((int)(this.Width/2 - totalPathWidth/2), 160);
            btnSelectPath.moveRight(txtPath, 5);

            lblCmt1.moveBelow(txtPath, 38 + 29);
            lblCmt1.Left = this.Width / 2 - lblCmt1.Width / 2;

            lblCmt2.moveBelow(lblCmt1, 5);
            lblCmt2.Left = lblCmt1.Left;

            lblCmt3.moveBelow(lblCmt2, 13);
            lblCmt3.Left = lblCmt1.Left;

            btnStart.moveBelow(lblCmt3, 46);
            btnStart.Left = this.Width / 2 - btnStart.Width / 2;
        }

        void makeBgImage()
        {
            Graphics g = getDC();

            Bitmap bmpTopImg = Properties.Resources.img_backup_disk;        //상단 CD이미지
            Bitmap bmpTitleText = Properties.Resources.txt_backup_setspace; //프로젝트를 백업할 공간을 지정해 주세요

            //상단 CD이미지
            g.DrawImage(bmpTopImg, this.Width/2 - bmpTopImg.Width/2, 0);

            //프로젝트를 백업할 공간을 지정해 주세요
            g.DrawImage(bmpTitleText, this.Width / 2 - bmpTitleText.Width / 2, bmpTopImg.Height+67);
        }

        public void goBack()
        {
        }

        public void goRefresh()
        {
        }
    }
}
