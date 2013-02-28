namespace ClingClient.commitList
{
    partial class ProjectNamePanel
    {
        /// <summary> 
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.nameEditBox = new System.Windows.Forms.TextBox();
            this.nameLabel = new ClingClient.controls.ShadowLabel();
            this.SuspendLayout();
            // 
            // nameEditBox
            // 
            this.nameEditBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.nameEditBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(63)))), ((int)(((byte)(63)))), ((int)(((byte)(63)))));
            this.nameEditBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nameEditBox.Font = new System.Drawing.Font("맑은 고딕", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.nameEditBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.nameEditBox.Location = new System.Drawing.Point(0, 4);
            this.nameEditBox.MaxLength = 200;
            this.nameEditBox.Name = "nameEditBox";
            this.nameEditBox.Size = new System.Drawing.Size(529, 33);
            this.nameEditBox.TabIndex = 40;
            this.nameEditBox.Visible = false;
            this.nameEditBox.VisibleChanged += new System.EventHandler(this.nameEditBox_VisibleChanged);
            this.nameEditBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.nameEditBox_KeyDown);
            this.nameEditBox.Leave += new System.EventHandler(this.nameEditBox_Leave);
            // 
            // nameLabel
            // 
            this.nameLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.nameLabel.AutoEllipsis = true;
            this.nameLabel.BackColor = System.Drawing.Color.Transparent;
            this.nameLabel.Font = new System.Drawing.Font("맑은 고딕", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.nameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.nameLabel.Location = new System.Drawing.Point(0, 8);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(21)))), ((int)(((byte)(22)))));
            this.nameLabel.Size = new System.Drawing.Size(233, 25);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.xOffsetShadow = 0F;
            this.nameLabel.yOffsetShadow = -1F;
            this.nameLabel.SizeChanged += new System.EventHandler(this.nameLabel_SizeChanged);
            this.nameLabel.MouseEnter += new System.EventHandler(this.nameLabel_MouseEnter);
            // 
            // ProjectNamePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameEditBox);
            this.DoubleBuffered = true;
            this.Name = "ProjectNamePanel";
            this.Size = new System.Drawing.Size(529, 41);
            this.SizeChanged += new System.EventHandler(this.ProjectNamePanel_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ProjectNamePanel_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ProjectNamePanel_MouseClick);
            this.MouseLeave += new System.EventHandler(this.ProjectNamePanel_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ProjectNamePanel_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameEditBox;
        public controls.ShadowLabel nameLabel;
    }
}
