namespace ClingClient.controls {
    partial class RepoStatusView {
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
            this.repoStatusLabel = new System.Windows.Forms.PictureBox();
            this.localStatusButton = new ClingClient.ClingButton();
            ((System.ComponentModel.ISupportInitialize)(this.repoStatusLabel)).BeginInit();
            this.SuspendLayout();
            // 
            // repoStatusLabel
            // 
            this.repoStatusLabel.Location = new System.Drawing.Point(29, 7);
            this.repoStatusLabel.Name = "repoStatusLabel";
            this.repoStatusLabel.Size = new System.Drawing.Size(98, 12);
            this.repoStatusLabel.TabIndex = 1;
            this.repoStatusLabel.TabStop = false;
            // 
            // localStatusButton
            // 
            this.localStatusButton.BackColor = System.Drawing.Color.Transparent;
            this.localStatusButton.BackgroundImage = global::ClingClient.Properties.Resources.ico_state_record_on;
            this.localStatusButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.localStatusButton.bmpIn = null;
            this.localStatusButton.bmpNormal = global::ClingClient.Properties.Resources.ico_state_record_on;
            this.localStatusButton.bmpPress = null;
            this.localStatusButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.localStatusButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.localStatusButton.Location = new System.Drawing.Point(0, 0);
            this.localStatusButton.Name = "localStatusButton";
            this.localStatusButton.Size = new System.Drawing.Size(26, 27);
            this.localStatusButton.TabIndex = 0;
            this.localStatusButton.TabStop = false;
            this.localStatusButton.text = null;
            // 
            // RepoStatusView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.repoStatusLabel);
            this.Controls.Add(this.localStatusButton);
            this.DoubleBuffered = true;
            this.Name = "RepoStatusView";
            this.Size = new System.Drawing.Size(129, 25);
            ((System.ComponentModel.ISupportInitialize)(this.repoStatusLabel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox repoStatusLabel;
        public ClingButton localStatusButton;
    }
}
