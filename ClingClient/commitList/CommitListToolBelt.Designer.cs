namespace ClingClient.commitList {
    partial class CommitListToolBelt {
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
            this.viewByListImage = new System.Windows.Forms.PictureBox();
            this.viewByTreeImage = new System.Windows.Forms.PictureBox();
            this.viewTypeToggle = new ClingClient.ClingCheckBox();
            this.status = new ClingClient.controls.ShadowLabel();
            ((System.ComponentModel.ISupportInitialize)(this.viewByListImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewByTreeImage)).BeginInit();
            this.SuspendLayout();
            // 
            // viewByListImage
            // 
            this.viewByListImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewByListImage.Image = global::ClingClient.Properties.Resources.txt_sort_list_on;
            this.viewByListImage.Location = new System.Drawing.Point(596, 12);
            this.viewByListImage.Name = "viewByListImage";
            this.viewByListImage.Size = new System.Drawing.Size(31, 12);
            this.viewByListImage.TabIndex = 2;
            this.viewByListImage.TabStop = false;
            // 
            // viewByTreeImage
            // 
            this.viewByTreeImage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewByTreeImage.Image = global::ClingClient.Properties.Resources.txt_sort_tree_disable;
            this.viewByTreeImage.Location = new System.Drawing.Point(676, 12);
            this.viewByTreeImage.Name = "viewByTreeImage";
            this.viewByTreeImage.Size = new System.Drawing.Size(31, 12);
            this.viewByTreeImage.TabIndex = 3;
            this.viewByTreeImage.TabStop = false;
            // 
            // viewTypeToggle
            // 
            this.viewTypeToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.viewTypeToggle.BackColor = System.Drawing.Color.Transparent;
            this.viewTypeToggle.BackgroundImage = global::ClingClient.Properties.Resources.btn_sort_left;
            this.viewTypeToggle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.viewTypeToggle.bmpOff = global::ClingClient.Properties.Resources.btn_sort_left;
            this.viewTypeToggle.bmpOn = global::ClingClient.Properties.Resources.btn_sort_right;
            this.viewTypeToggle.checkValue = false;
            this.viewTypeToggle.Location = new System.Drawing.Point(630, 8);
            this.viewTypeToggle.Name = "viewTypeToggle";
            this.viewTypeToggle.Size = new System.Drawing.Size(43, 21);
            this.viewTypeToggle.TabIndex = 1;
            this.viewTypeToggle.TabStop = false;
            this.viewTypeToggle.CheckChanged += new System.EventHandler(this.viewTypeToggle_CheckChanged);
            // 
            // status
            // 
            this.status.Font = new System.Drawing.Font("돋움", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.status.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.status.Location = new System.Drawing.Point(43, 12);
            this.status.Name = "status";
            this.status.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
            this.status.Size = new System.Drawing.Size(150, 11);
            this.status.TabIndex = 0;
            this.status.xOffsetShadow = 0F;
            this.status.yOffsetShadow = -1F;
            // 
            // CommitListToolBelt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.viewByTreeImage);
            this.Controls.Add(this.viewByListImage);
            this.Controls.Add(this.viewTypeToggle);
            this.Controls.Add(this.status);
            this.DoubleBuffered = true;
            this.Name = "CommitListToolBelt";
            this.Size = new System.Drawing.Size(727, 36);
            ((System.ComponentModel.ISupportInitialize)(this.viewByListImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.viewByTreeImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private controls.ShadowLabel status;
        private ClingCheckBox viewTypeToggle;
        private System.Windows.Forms.PictureBox viewByListImage;
        private System.Windows.Forms.PictureBox viewByTreeImage;
    }
}
