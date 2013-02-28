namespace ClingClient.forms {
    partial class frmUpdatePopup {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmUpdatePopup));
            this.updateTitle = new System.Windows.Forms.Label();
            this.versionTitle = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.updateButton = new ClingClient.ClingButton();
            this.skipButton = new ClingClient.ClingButton();
            this.importantIcon = new System.Windows.Forms.PictureBox();
            this.updatingNotice = new System.Windows.Forms.Label();
            this.updateProgress = new ClingClient.ClingProgress();
            this.cancelButton = new ClingClient.ClingButton();
            this.releaseNotes = new System.Windows.Forms.WebBrowser();
            this.noticePanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.importantIcon)).BeginInit();
            this.noticePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateTitle
            // 
            this.updateTitle.AutoSize = true;
            this.updateTitle.BackColor = System.Drawing.Color.Transparent;
            this.updateTitle.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.updateTitle.Location = new System.Drawing.Point(0, 2);
            this.updateTitle.Name = "updateTitle";
            this.updateTitle.Size = new System.Drawing.Size(176, 12);
            this.updateTitle.TabIndex = 1;
            this.updateTitle.Text = "설치할 업데이트가 있습니다.";
            // 
            // versionTitle
            // 
            this.versionTitle.AutoSize = true;
            this.versionTitle.BackColor = System.Drawing.Color.Transparent;
            this.versionTitle.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.versionTitle.Location = new System.Drawing.Point(0, 18);
            this.versionTitle.Name = "versionTitle";
            this.versionTitle.Size = new System.Drawing.Size(61, 12);
            this.versionTitle.TabIndex = 2;
            this.versionTitle.Text = "최신버전 :";
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.BackColor = System.Drawing.Color.Transparent;
            this.version.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.version.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(143)))), ((int)(((byte)(213)))));
            this.version.Location = new System.Drawing.Point(74, 18);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(36, 12);
            this.version.TabIndex = 3;
            this.version.Text = "0.1.0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(209)))), ((int)(((byte)(209)))), ((int)(((byte)(209)))));
            this.pictureBox1.Location = new System.Drawing.Point(2, 299);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(406, 1);
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // updateButton
            // 
            this.updateButton.BackColor = System.Drawing.Color.Transparent;
            this.updateButton.BackgroundImage = global::ClingClient.Properties.Resources.btn_update_normal;
            this.updateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.updateButton.bmpIn = null;
            this.updateButton.bmpNormal = global::ClingClient.Properties.Resources.btn_update_normal;
            this.updateButton.bmpPress = global::ClingClient.Properties.Resources.btn_update_press;
            this.updateButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.updateButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.updateButton.Location = new System.Drawing.Point(225, 314);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(80, 31);
            this.updateButton.TabIndex = 6;
            this.updateButton.TabStop = false;
            this.updateButton.text = null;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // skipButton
            // 
            this.skipButton.BackColor = System.Drawing.Color.Transparent;
            this.skipButton.BackgroundImage = global::ClingClient.Properties.Resources.btn_update_delay_normal;
            this.skipButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.skipButton.bmpIn = null;
            this.skipButton.bmpNormal = global::ClingClient.Properties.Resources.btn_update_delay_normal;
            this.skipButton.bmpPress = global::ClingClient.Properties.Resources.btn_update_delay_press;
            this.skipButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.skipButton.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.skipButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.skipButton.Location = new System.Drawing.Point(311, 314);
            this.skipButton.Name = "skipButton";
            this.skipButton.Size = new System.Drawing.Size(66, 31);
            this.skipButton.TabIndex = 7;
            this.skipButton.TabStop = false;
            this.skipButton.text = "";
            this.skipButton.Click += new System.EventHandler(this.skipButton_Click);
            // 
            // importantIcon
            // 
            this.importantIcon.BackColor = System.Drawing.Color.Transparent;
            this.importantIcon.Image = ((System.Drawing.Image)(resources.GetObject("importantIcon.Image")));
            this.importantIcon.Location = new System.Drawing.Point(35, 43);
            this.importantIcon.Name = "importantIcon";
            this.importantIcon.Size = new System.Drawing.Size(34, 30);
            this.importantIcon.TabIndex = 8;
            this.importantIcon.TabStop = false;
            this.importantIcon.Visible = false;
            // 
            // updatingNotice
            // 
            this.updatingNotice.AutoSize = true;
            this.updatingNotice.BackColor = System.Drawing.Color.Transparent;
            this.updatingNotice.Location = new System.Drawing.Point(31, 363);
            this.updatingNotice.Name = "updatingNotice";
            this.updatingNotice.Size = new System.Drawing.Size(209, 12);
            this.updatingNotice.TabIndex = 9;
            this.updatingNotice.Text = "업데이트 파일을 다운로드 중입니다...";
            // 
            // updateProgress
            // 
            this.updateProgress.BackColor = System.Drawing.Color.Transparent;
            this.updateProgress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.updateProgress.bmpBackLeft = null;
            this.updateProgress.bmpBackMiddle = null;
            this.updateProgress.bmpBackRight = null;
            this.updateProgress.bmpBackSingle = global::ClingClient.Properties.Resources.bg_update_progress;
            this.updateProgress.bmpFrontLeft = global::ClingClient.Properties.Resources.img_update_pregress_l;
            this.updateProgress.bmpFrontMiddle = global::ClingClient.Properties.Resources.img_update_pregress_m;
            this.updateProgress.bmpFrontRight = global::ClingClient.Properties.Resources.img_update_pregress_r;
            this.updateProgress.currentValue = 0D;
            this.updateProgress.Location = new System.Drawing.Point(29, 389);
            this.updateProgress.Name = "updateProgress";
            this.updateProgress.Size = new System.Drawing.Size(347, 9);
            this.updateProgress.TabIndex = 10;
            // 
            // cancelButton
            // 
            this.cancelButton.BackColor = System.Drawing.Color.Transparent;
            this.cancelButton.BackgroundImage = global::ClingClient.Properties.Resources.btn_popup_cancel_normal;
            this.cancelButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cancelButton.bmpIn = null;
            this.cancelButton.bmpNormal = global::ClingClient.Properties.Resources.btn_popup_cancel_normal;
            this.cancelButton.bmpPress = global::ClingClient.Properties.Resources.btn_popup_cancel_press;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cancelButton.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.cancelButton.Location = new System.Drawing.Point(311, 314);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(66, 31);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.TabStop = false;
            this.cancelButton.text = "";
            this.cancelButton.Visible = false;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // releaseNotes
            // 
            this.releaseNotes.AllowNavigation = false;
            this.releaseNotes.IsWebBrowserContextMenuEnabled = false;
            this.releaseNotes.Location = new System.Drawing.Point(31, 88);
            this.releaseNotes.MinimumSize = new System.Drawing.Size(20, 20);
            this.releaseNotes.Name = "releaseNotes";
            this.releaseNotes.Size = new System.Drawing.Size(346, 178);
            this.releaseNotes.TabIndex = 13;
            this.releaseNotes.WebBrowserShortcutsEnabled = false;
            this.releaseNotes.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.releaseNotes_DocumentCompleted);
            // 
            // noticePanel
            // 
            this.noticePanel.BackColor = System.Drawing.Color.Transparent;
            this.noticePanel.Controls.Add(this.version);
            this.noticePanel.Controls.Add(this.updateTitle);
            this.noticePanel.Controls.Add(this.versionTitle);
            this.noticePanel.Location = new System.Drawing.Point(75, 41);
            this.noticePanel.Name = "noticePanel";
            this.noticePanel.Size = new System.Drawing.Size(254, 41);
            this.noticePanel.TabIndex = 14;
            // 
            // frmUpdatePopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 420);
            this.Controls.Add(this.releaseNotes);
            this.Controls.Add(this.updateProgress);
            this.Controls.Add(this.updatingNotice);
            this.Controls.Add(this.importantIcon);
            this.Controls.Add(this.skipButton);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.noticePanel);
            this.DoubleBuffered = true;
            this.Name = "frmUpdatePopup";
            this.showCloseButton = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmUpdate_FormClosing);
            this.Load += new System.EventHandler(this.frmUpdate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.importantIcon)).EndInit();
            this.noticePanel.ResumeLayout(false);
            this.noticePanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label updateTitle;
        private System.Windows.Forms.Label versionTitle;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.PictureBox pictureBox1;
        private ClingButton updateButton;
        private ClingButton skipButton;
        private System.Windows.Forms.PictureBox importantIcon;
        private System.Windows.Forms.Label updatingNotice;
        private ClingProgress updateProgress;
        private ClingButton cancelButton;
        private System.Windows.Forms.WebBrowser releaseNotes;
        private System.Windows.Forms.Panel noticePanel;
    }
}