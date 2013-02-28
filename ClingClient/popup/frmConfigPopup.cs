using System;
using System.Drawing;
using System.Windows.Forms;
using ClingClientEngine;

namespace ClingClient.forms {
    public partial class frmConfigPopup : frmCommonPopup 
    {
        ClingButton btnOK;
        ClingButton btnCancel;
        ClingButton btnSearch;
        ClingTextBox txtBackupDir;

        public static int exitFlag = 0;     //1:로그아웃 2:계정변경 3:OK
        public static string exitString = "";

        public frmConfigPopup()
        {
            InitializeComponent();
            setControls();

            showCloseButton = true;
            showButtonLine = true;

            this.ShowInTaskbar = false;
            this.Size = new Size(475, 368);
            this.Resize += new EventHandler(frmAbout_Resize);
        }

        void frmAbout_Resize(object sender, EventArgs e)
        {
            btnSearch.moveParentTopRight(this, 31, 27 + 27);

            //취소버튼
            btnCancel.moveParentBottomRight(this, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);

            //등록
            btnOK.moveLeft(btnCancel, 5);

            txtBackupDir.moveLeft(btnSearch, 3);
        }

        void setControls()
        {
            btnSearch = new ClingButton();
            btnSearch.setImages(Properties.Resources.btn_setting_search_normal,
                Properties.Resources.btn_setting_search_press,
                Properties.Resources.btn_setting_search_hover);
            btnSearch.ButtonClick += new EventHandler(btnSearch_ButtonClick);

            //취소버튼
            btnCancel = new ClingButton();
            btnCancel.setImages(Properties.Resources.btn_popup_cancel_normal,
                Properties.Resources.btn_popup_cancel_press,
                Properties.Resources.btn_popup_cancel_normal);
            btnCancel.ButtonClick += new EventHandler(btnCancel_ButtonClick);


            //등록
            btnOK = new ClingButton();
            btnOK.setImages(Properties.Resources.btn_popup_ok_norml,
                Properties.Resources.btn_popup_ok_press,
                Properties.Resources.btn_popup_ok_norml);
            btnOK.ButtonClick += new EventHandler(btnOK_ButtonClick);

            txtBackupDir = new ClingTextBox("백업폴더");
            txtBackupDir.Size = new Size(260, 26);
            txtBackupDir.setText(EngineConfig.repositoryContainerPath);


            this.Controls.Add(btnSearch);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.Controls.Add(txtBackupDir);

            this.AcceptButton = btnOK;
        }

        //디렉토리 선택
        void btnSearch_ButtonClick(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                String selectedDir = dlg.SelectedPath;
                if (!string.IsNullOrWhiteSpace(selectedDir))
                {
                    txtBackupDir.setText(selectedDir);
                }
            }
        }

        //계정변경
        void btnChangeAccount_ButtonClick(object sender, EventArgs e)
        {
            exitFlag = 2;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        //로그아웃
        void btnLogout_ButtonClick(object sender, EventArgs e)
        {
            exitFlag = 1;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        //OK
        void btnOK_ButtonClick(object sender, EventArgs e)
        {
            if (!Program.mainframe.prepareBackupDir(txtBackupDir.getText())) return;

            exitString = txtBackupDir.getText();
            exitFlag = 3;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        void btnCancel_ButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        protected override void makeBackgroundImage(Graphics g)
        {
            //title
            Font fontCmt1 = new System.Drawing.Font("맑은 고딕", 12F, FontStyle.Regular, GraphicsUnit.Pixel);
            g.DrawString("환경설정", fontCmt1, new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), 8, 5);


            //상단 가로선
            float topHLine = 27;
            g.DrawLine(new Pen(Color.FromArgb(0xE0, 0xE0, 0xE0), 1F), 2, topHLine, this.Width - 2, topHLine);
            g.DrawLine(new Pen(Color.White, 1F), 2, topHLine+1, this.Width - 2, topHLine+1);


            //백업폴더
            float curTop = topHLine+25+7;
            Font fontCmt2 = new System.Drawing.Font("돋음", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString("백업폴더", fontCmt2, new SolidBrush(Color.FromArgb(0x33, 0x33, 0x33)), 31, curTop);

        }
    }
}
