using System;
using ClingClient.forms;
using ClingClientEngine;
using System.Windows.Forms;

namespace ClingClient.popup
{
    class frmProgress : frmCommonPopup, ProgressIF
    {
        ProgressBar prgBar;
        ListBox lvFile;
        ClingLabel lblTitle;
        string title;
        bool progressWithMsg;

        delegate void ADDLOG(string msg);
        ADDLOG addlog = null;

        delegate void SETPRG(double val);
        SETPRG setprg = null;

        public RepositoryContext.GeneralProgressDelegate progressValueChanged;

        public delegate void AFTERWORK(object param);
        public event EventHandler CancelClick;

        public frmProgress(string title, bool progressWithMsg = true)
        {
            Console.WriteLine("frmProgress created!");

            this.progressWithMsg = progressWithMsg;
            this.title = title;
            addlog = new ADDLOG(_addLog);
            setprg = new SETPRG(_setPrg);

            progressValueChanged = new RepositoryContext.GeneralProgressDelegate(_progressValueChanged);

            //this.TopMost = true;
            this.Size = progressWithMsg ? new System.Drawing.Size(300, 200) : new System.Drawing.Size(300, 80);
            this.Load += new EventHandler(frmProgress_Load);
            setControls();
        }

        void frmProgress_Load(object sender, EventArgs e)
        {
            //취소 가능하게 close 버튼 처리!
            this.showCloseButton = true;
            this.btnClose.ButtonClick -= base.btnClose_ButtonClick; //창 닫아버리는 원래 핸들러 제거
            this.btnClose.ButtonClick += new EventHandler(btnCancelButtonClick);
            this.btnClose.MouseEnter += new EventHandler(btnClose_MouseEnter);
        }

        void btnClose_MouseEnter(object sender, EventArgs e)
        {
            //Console.WriteLine("btnClose_MouseEnter");
            Cursor.Current = Cursors.Hand;
        }

        void btnCancelButtonClick(object sender, EventArgs e)
        {
            //Console.WriteLine("btnClose_ButtonClick");

            if (CancelClick != null)
            {
                CancelClick(this, null);
            }
        }

        public void setTitle(string title)
        {
            lblTitle.label.Text = title;
        }

        void setControls()
        {
            int margin = 20;

            lblTitle = new ClingLabel(title);
            lblTitle.label.Font = new System.Drawing.Font("돋음", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            lblTitle.Location = new System.Drawing.Point(margin, 15);


            prgBar = new ProgressBar();
            prgBar.Maximum = 100;
            prgBar.Minimum = 0;
            prgBar.Style = ProgressBarStyle.Continuous;
            prgBar.Width = this.Width - margin * 2;
            prgBar.Height = 10;
            prgBar.BackColor = System.Drawing.Color.LightGray;
            
            lvFile = new ListBox();
            lvFile.Location = new System.Drawing.Point(margin, lblTitle.Top + lblTitle.Height + margin);
            lvFile.Size = new System.Drawing.Size(prgBar.Width, this.Height - lblTitle.Height - margin * 2 - prgBar.Height - 5);
            lvFile.Cursor = Cursors.WaitCursor;

            prgBar.Left = margin;
            if (progressWithMsg)
            {
                this.Controls.Add(lvFile);
                prgBar.Top = lvFile.Top + lvFile.Height - 5;
            }
            else
            {
                prgBar.Top = this.Height - prgBar.Height - 15;
            }
            
            this.Controls.Add(lblTitle);
            this.Controls.Add(prgBar);
        }

        public void sendMsg(string msg, double val)
        {
            this.Invoke(addlog, new object[] { msg });
            this.Invoke(setprg, new object[] { val });
        }

        void _addLog(string msg)
        {
            //너무 많이 쌓이면 뻗을수도 있으므로,, 클리어!
            if (lvFile.Items.Count > 1000) lvFile.Items.Clear();

            lvFile.Items.Add(msg);
            lvFile.TopIndex = lvFile.Items.Count - 1;
        }

        void _setPrg(double val)
        {
            prgBar.Value = (int)val;
        }

        void _progressValueChanged(double val)
        {
            this.Invoke(setprg, new object[] { val });
        }
    }
}
