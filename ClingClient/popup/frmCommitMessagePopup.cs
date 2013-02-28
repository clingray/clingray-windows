using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ClingClient.forms
{
    public partial class frmCommitMessagePopup : frmCommonPopup
    {
        public string repositoryName { get; set; }
        public ClingTextBox txtCommitMsg;
        ClingButton btnCancel;
        ClingButton btnRegister;

        public frmCommitMessagePopup()
        {
            InitializeComponent();
            setControls();

            showClingLogo = true;
            showCloseButton = true;
            showButtonLine = true;

            this.Resize += new EventHandler(frmCommitMessagePopup_Resize);
            this.Load += new EventHandler(frmCommitMessagePopup_Load);
        }

        void frmCommitMessagePopup_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(402, 197);
            this.BringToFront();
            this.TopMost = true;

            // Ensure popup is focused.
            this.Activate();
        }

        void setControls()
        {
            //커밋 메시지
            txtCommitMsg = new ClingTextBox("지금 이 곡의 작업단계는? 예) 기타 녹음 완료, 최종 믹싱 진행중");
            //txtCommitMsg.Font = new System.Drawing.Font("돋음", 14F, GraphicsUnit.Pixel);
            txtCommitMsg.Size = new System.Drawing.Size(340, 26);
            txtCommitMsg.TabIndex = 0;

            //취소버튼
            btnCancel = new ClingButton();
            btnCancel.setImages(Properties.Resources.btn_popup_cancel_normal,
                Properties.Resources.btn_popup_cancel_press,
                Properties.Resources.btn_popup_cancel_normal);
            btnCancel.ButtonClick += new EventHandler(btnCancel_ButtonClick);

            //등록
            btnRegister = new ClingButton();
            btnRegister.setImages(Properties.Resources.btn_popup_ok_norml,
                Properties.Resources.btn_popup_ok_press,
                Properties.Resources.btn_popup_ok_norml);
            btnRegister.ButtonClick += new EventHandler(btnRegister_ButtonClick);

            this.Controls.Add(txtCommitMsg);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnRegister);

            this.AcceptButton = btnRegister;
        }

        void btnRegister_ButtonClick(object sender, EventArgs e)
        {
            if (txtCommitMsg.getText().Length == 0)
            {
                frmAlertPopup.alert("메시지를 입력하세요.");
                return;
            }

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        void btnCancel_ButtonClick(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        void frmCommitMessagePopup_Resize(object sender, EventArgs e)
        {
            Point ptTitle = getContentTopLocation();

            //커밋 메시지
            txtCommitMsg.Location = new Point(ptTitle.X, (int)(ptTitle.Y + 25));

            //취소버튼
            btnCancel.moveParentBottomRight(this, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);

            //등록
            btnRegister.moveLeft(btnCancel, 5);
        }

        protected override void makeBackgroundImage(Graphics g)
        {
            //타이틀
            {
                string title = "현재 작업상태 기록";
                if (!string.IsNullOrWhiteSpace(repositoryName))
                {
                    title = "[" + repositoryName + "] 의 " + title;
                }

                Font fontCmt1 = new System.Drawing.Font("돋음", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawString(title, fontCmt1, new SolidBrush(Color.Black), getContentTopLocation());
            }
        }
    }
}
