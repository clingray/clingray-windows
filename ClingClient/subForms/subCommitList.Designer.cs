using ClingClient.controls;
namespace ClingClient.subForms {
    partial class subCommitList {
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
            this.separatorBar = new System.Windows.Forms.PictureBox();
            this.coverBG = new System.Windows.Forms.PictureBox();
            this.name = new ClingClient.commitList.ProjectNamePanel();
            this.toolBelt = new ClingClient.commitList.CommitListToolBelt();
            this.repoStatusView = new ClingClient.controls.RepoStatusView();
            this.commitButton = new ClingClient.ClingButton();
            this.creationLabel = new ClingClient.controls.ShadowLabel();
            this.lastModifiedLabel = new ClingClient.controls.ShadowLabel();
            this.label3 = new ClingClient.controls.ShadowLabel();
            this.label1 = new ClingClient.controls.ShadowLabel();
            this.tableView = new ClingClient.controls.ClingTableView();
            ((System.ComponentModel.ISupportInitialize)(this.separatorBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.coverBG)).BeginInit();
            this.SuspendLayout();
            // 
            // separatorBar
            // 
            this.separatorBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.separatorBar.BackgroundImage = global::ClingClient.Properties.Resources.line_list;
            this.separatorBar.Location = new System.Drawing.Point(45, 175);
            this.separatorBar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.separatorBar.Name = "separatorBar";
            this.separatorBar.Size = new System.Drawing.Size(639, 2);
            this.separatorBar.TabIndex = 40;
            this.separatorBar.TabStop = false;
            // 
            // coverBG
            // 
            this.coverBG.BackColor = System.Drawing.Color.Transparent;
            this.coverBG.Location = new System.Drawing.Point(29, 51);
            this.coverBG.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.coverBG.Name = "coverBG";
            this.coverBG.Size = new System.Drawing.Size(126, 109);
            this.coverBG.TabIndex = 29;
            this.coverBG.TabStop = false;
            // 
            // name
            // 
            this.name.BackColor = System.Drawing.Color.Transparent;
            this.name.Location = new System.Drawing.Point(161, 58);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(370, 36);
            this.name.TabIndex = 0;
            this.name.TabStop = false;
            // 
            // toolBelt
            // 
            this.toolBelt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toolBelt.BackColor = System.Drawing.Color.Transparent;
            this.toolBelt.Location = new System.Drawing.Point(0, 0);
            this.toolBelt.Name = "toolBelt";
            this.toolBelt.repositoryStatus = "";
            this.toolBelt.Size = new System.Drawing.Size(727, 36);
            this.toolBelt.TabIndex = 42;
            this.toolBelt.TabStop = false;
            this.toolBelt.viewType = ClingClient.commitList.CommitListToolBelt.ViewType.LIST;
            // 
            // repoStatusView
            // 
            this.repoStatusView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.repoStatusView.BackColor = System.Drawing.Color.Transparent;
            this.repoStatusView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.repoStatusView.Location = new System.Drawing.Point(553, 117);
            this.repoStatusView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.repoStatusView.Name = "repoStatusView";
            this.repoStatusView.repositoryStatus = ClingClientEngine.RepositoryContext.Status.UNKNOWN;
            this.repoStatusView.Size = new System.Drawing.Size(129, 32);
            this.repoStatusView.TabIndex = 39;
            this.repoStatusView.TabStop = false;
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.BackColor = System.Drawing.Color.Transparent;
            this.commitButton.BackgroundImage = global::ClingClient.Properties.Resources.btn_write_record_normal;
            this.commitButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.commitButton.bmpIn = null;
            this.commitButton.bmpNormal = global::ClingClient.Properties.Resources.btn_write_record_normal;
            this.commitButton.bmpPress = global::ClingClient.Properties.Resources.btn_write_record_press;
            this.commitButton.DialogResult = System.Windows.Forms.DialogResult.None;
            this.commitButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(5)))), ((int)(((byte)(5)))));
            this.commitButton.Location = new System.Drawing.Point(548, 64);
            this.commitButton.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size(128, 47);
            this.commitButton.TabIndex = 37;
            this.commitButton.TabStop = false;
            this.commitButton.text = null;
            this.commitButton.Click += new System.EventHandler(this.commitButton_Click);
            // 
            // creationLabel
            // 
            this.creationLabel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.creationLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.creationLabel.Location = new System.Drawing.Point(238, 118);
            this.creationLabel.Name = "creationLabel";
            this.creationLabel.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(40)))));
            this.creationLabel.Size = new System.Drawing.Size(140, 12);
            this.creationLabel.TabIndex = 36;
            this.creationLabel.xOffsetShadow = 1F;
            this.creationLabel.yOffsetShadow = 1F;
            // 
            // lastModifiedLabel
            // 
            this.lastModifiedLabel.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.lastModifiedLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lastModifiedLabel.Location = new System.Drawing.Point(238, 98);
            this.lastModifiedLabel.Name = "lastModifiedLabel";
            this.lastModifiedLabel.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(40)))));
            this.lastModifiedLabel.Size = new System.Drawing.Size(120, 12);
            this.lastModifiedLabel.TabIndex = 34;
            this.lastModifiedLabel.xOffsetShadow = 1F;
            this.lastModifiedLabel.yOffsetShadow = 1F;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("돋움", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(129)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(116)))), ((int)(((byte)(120)))));
            this.label3.Location = new System.Drawing.Point(163, 119);
            this.label3.Name = "label3";
            this.label3.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(40)))));
            this.label3.Size = new System.Drawing.Size(73, 12);
            this.label3.TabIndex = 33;
            this.label3.Text = "프로젝트 생성";
            this.label3.xOffsetShadow = 0F;
            this.label3.yOffsetShadow = -1F;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("돋움", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(115)))), ((int)(((byte)(116)))), ((int)(((byte)(120)))));
            this.label1.Location = new System.Drawing.Point(163, 99);
            this.label1.Name = "label1";
            this.label1.shadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(40)))));
            this.label1.Size = new System.Drawing.Size(73, 12);
            this.label1.TabIndex = 31;
            this.label1.Text = "마지막 기록";
            this.label1.xOffsetShadow = 0F;
            this.label1.yOffsetShadow = -1F;
            // 
            // tableView
            // 
            this.tableView.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.tableView.BackColor = System.Drawing.Color.Transparent;
            this.tableView.cellMarginHeight = 6;
            this.tableView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.tableView.Font = new System.Drawing.Font("돋움", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tableView.Location = new System.Drawing.Point(28, 191);
            this.tableView.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableView.Name = "tableView";
            this.tableView.Size = new System.Drawing.Size(174, 174);
            this.tableView.TabIndex = 28;
            this.tableView.TabStop = false;
            // 
            // subCommitList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.Controls.Add(this.name);
            this.Controls.Add(this.toolBelt);
            this.Controls.Add(this.separatorBar);
            this.Controls.Add(this.repoStatusView);
            this.Controls.Add(this.commitButton);
            this.Controls.Add(this.creationLabel);
            this.Controls.Add(this.lastModifiedLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.coverBG);
            this.Controls.Add(this.tableView);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "subCommitList";
            this.Size = new System.Drawing.Size(727, 617);
            this.Load += new System.EventHandler(this.subCommitList_Load);
            this.Click += new System.EventHandler(this.subCommitList_Click);
            ((System.ComponentModel.ISupportInitialize)(this.separatorBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.coverBG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox separatorBar;
        private RepoStatusView repoStatusView;
        private ClingButton commitButton;
        private ShadowLabel creationLabel;
        private ShadowLabel lastModifiedLabel;
        private ShadowLabel label3;
        private ShadowLabel label1;
        private System.Windows.Forms.PictureBox coverBG;
        private ClingTableView tableView;
        private commitList.CommitListToolBelt toolBelt;
        private commitList.ProjectNamePanel name;

    }
}
