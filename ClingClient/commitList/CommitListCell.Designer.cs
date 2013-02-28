namespace ClingClient.controls {
    partial class CommitListCell {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent() {
            this.viewFilesButton = new ClingClient.ClingButton();
            this.checkoutButton = new ClingClient.ClingButton();
            this.currentIndicator = new System.Windows.Forms.PictureBox();
            this.panel = new System.Windows.Forms.Panel();
            this.timestampLabel = new System.Windows.Forms.Label();
            this.commitMessage = new System.Windows.Forms.Label();
            this.picSeparator = new System.Windows.Forms.PictureBox();
            this.picDay = new System.Windows.Forms.PictureBox();
            this.picMonth = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.currentIndicator)).BeginInit();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSeparator)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonth)).BeginInit();
            this.SuspendLayout();
            // 
            // viewFilesButton
            // 
            this.viewFilesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewFilesButton.BackColor = System.Drawing.Color.Transparent;
            this.viewFilesButton.BackgroundImage = global::ClingClient.Properties.Resources.btn_list_file_normal;
            this.viewFilesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.viewFilesButton.bmpIn = global::ClingClient.Properties.Resources.btn_list_file_hover;
            this.viewFilesButton.bmpNormal = global::ClingClient.Properties.Resources.btn_list_file_normal;
            this.viewFilesButton.bmpPress = global::ClingClient.Properties.Resources.btn_list_file_press;
            this.viewFilesButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.viewFilesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.viewFilesButton.Location = new System.Drawing.Point(497, 18);
            this.viewFilesButton.Name = "viewFilesButton";
            this.viewFilesButton.Size = new System.Drawing.Size(66, 31);
            this.viewFilesButton.TabIndex = 15;
            this.viewFilesButton.TabStop = false;
            this.viewFilesButton.text = null;
            this.viewFilesButton.Visible = false;
            // 
            // checkoutButton
            // 
            this.checkoutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkoutButton.BackColor = System.Drawing.Color.Transparent;
            this.checkoutButton.BackgroundImage = global::ClingClient.Properties.Resources.btn_treeview_openh_normal;
            this.checkoutButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.checkoutButton.bmpIn = global::ClingClient.Properties.Resources.btn_treeview_openh_hover;
            this.checkoutButton.bmpNormal = global::ClingClient.Properties.Resources.btn_treeview_openh_normal;
            this.checkoutButton.bmpPress = global::ClingClient.Properties.Resources.btn_treeview_openh_down;
            this.checkoutButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.checkoutButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.checkoutButton.Location = new System.Drawing.Point(570, 18);
            this.checkoutButton.Name = "checkoutButton";
            this.checkoutButton.Size = new System.Drawing.Size(66, 31);
            this.checkoutButton.TabIndex = 13;
            this.checkoutButton.TabStop = false;
            this.checkoutButton.text = null;
            // 
            // currentIndicator
            // 
            this.currentIndicator.Image = global::ClingClient.Properties.Resources.img_listbar_now;
            this.currentIndicator.Location = new System.Drawing.Point(4, 6);
            this.currentIndicator.Name = "currentIndicator";
            this.currentIndicator.Size = new System.Drawing.Size(21, 53);
            this.currentIndicator.TabIndex = 7;
            this.currentIndicator.TabStop = false;
            this.currentIndicator.Visible = false;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.timestampLabel);
            this.panel.Controls.Add(this.commitMessage);
            this.panel.Controls.Add(this.picSeparator);
            this.panel.Controls.Add(this.picDay);
            this.panel.Controls.Add(this.picMonth);
            this.panel.Location = new System.Drawing.Point(35, 7);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(536, 51);
            this.panel.TabIndex = 14;
            // 
            // timestampLabel
            // 
            this.timestampLabel.AutoSize = true;
            this.timestampLabel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.timestampLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(118)))));
            this.timestampLabel.Location = new System.Drawing.Point(86, 31);
            this.timestampLabel.Name = "timestampLabel";
            this.timestampLabel.Size = new System.Drawing.Size(0, 13);
            this.timestampLabel.TabIndex = 12;
            // 
            // commitMessage
            // 
            this.commitMessage.AutoEllipsis = true;
            this.commitMessage.Font = new System.Drawing.Font("돋움", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.commitMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(2)))), ((int)(((byte)(2)))));
            this.commitMessage.Location = new System.Drawing.Point(87, 12);
            this.commitMessage.Name = "commitMessage";
            this.commitMessage.Size = new System.Drawing.Size(366, 12);
            this.commitMessage.TabIndex = 11;
            this.commitMessage.UseMnemonic = false;
            // 
            // picSeparator
            // 
            this.picSeparator.Image = global::ClingClient.Properties.Resources.div_list;
            this.picSeparator.Location = new System.Drawing.Point(68, 11);
            this.picSeparator.Name = "picSeparator";
            this.picSeparator.Size = new System.Drawing.Size(2, 33);
            this.picSeparator.TabIndex = 10;
            this.picSeparator.TabStop = false;
            // 
            // picDay
            // 
            this.picDay.Location = new System.Drawing.Point(10, 23);
            this.picDay.Name = "picDay";
            this.picDay.Size = new System.Drawing.Size(28, 20);
            this.picDay.TabIndex = 9;
            this.picDay.TabStop = false;
            // 
            // picMonth
            // 
            this.picMonth.Location = new System.Drawing.Point(10, 11);
            this.picMonth.Name = "picMonth";
            this.picMonth.Size = new System.Drawing.Size(28, 10);
            this.picMonth.TabIndex = 8;
            this.picMonth.TabStop = false;
            // 
            // CommitListCell
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.viewFilesButton);
            this.Controls.Add(this.checkoutButton);
            this.Controls.Add(this.currentIndicator);
            this.Controls.Add(this.panel);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.Name = "CommitListCell";
            this.Size = new System.Drawing.Size(652, 67);
            this.Load += new System.EventHandler(this.CommitListCell_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.CommitListCell_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.currentIndicator)).EndInit();
            this.panel.ResumeLayout(false);
            this.panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSeparator)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picMonth)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox currentIndicator;
        private System.Windows.Forms.Label timestampLabel;
        private System.Windows.Forms.Label commitMessage;
        private System.Windows.Forms.PictureBox picSeparator;
        private System.Windows.Forms.PictureBox picDay;
        private System.Windows.Forms.PictureBox picMonth;
        public ClingButton checkoutButton;
        private System.Windows.Forms.Panel panel;
        public ClingButton viewFilesButton;

    }
}
