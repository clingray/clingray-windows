namespace ClingClient
{
    partial class frmFrame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
            System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFrame));
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.currentRepoTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commitTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRepoTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentRepoTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkUpdateTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clingrayWebTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitTrayItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(195, 6);
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(195, 6);
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new System.Drawing.Size(195, 6);
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new System.Drawing.Size(195, 6);
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Clingray";
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.currentRepoTrayItem,
            this.commitTrayItem,
            this.openTrayItem,
            toolStripMenuItem1,
            this.createRepoTrayItem,
            this.recentRepoTrayItem,
            toolStripMenuItem2,
            this.helpTrayItem,
            this.checkUpdateTrayItem,
            this.clingrayWebTrayItem,
            toolStripMenuItem3,
            this.settingsTrayItem,
            toolStripMenuItem4,
            this.exitTrayItem});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.Size = new System.Drawing.Size(199, 248);
            // 
            // currentRepoTrayItem
            // 
            this.currentRepoTrayItem.Name = "currentRepoTrayItem";
            this.currentRepoTrayItem.Size = new System.Drawing.Size(198, 22);
            this.currentRepoTrayItem.Text = "기록 중 : 프로젝트명";
            this.currentRepoTrayItem.Visible = false;
            this.currentRepoTrayItem.Click += new System.EventHandler(this.trayItemCurrentRepo_Click);
            // 
            // commitTrayItem
            // 
            this.commitTrayItem.Name = "commitTrayItem";
            this.commitTrayItem.Size = new System.Drawing.Size(198, 22);
            this.commitTrayItem.Text = "최신 작업기록 남기기";
            this.commitTrayItem.Visible = false;
            this.commitTrayItem.Click += new System.EventHandler(this.trayItemCommit_Click);
            // 
            // openTrayItem
            // 
            this.openTrayItem.Font = new System.Drawing.Font("맑은 고딕", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.openTrayItem.Name = "openTrayItem";
            this.openTrayItem.Size = new System.Drawing.Size(198, 22);
            this.openTrayItem.Text = "Clingray 열기";
            this.openTrayItem.Click += new System.EventHandler(this.trayItemOpen_Click);
            // 
            // createRepoTrayItem
            // 
            this.createRepoTrayItem.Name = "createRepoTrayItem";
            this.createRepoTrayItem.Size = new System.Drawing.Size(198, 22);
            this.createRepoTrayItem.Text = "새 프로젝트 시작";
            this.createRepoTrayItem.Click += new System.EventHandler(this.trayItemCreateRepo_Click);
            // 
            // recentRepoTrayItem
            // 
            this.recentRepoTrayItem.Name = "recentRepoTrayItem";
            this.recentRepoTrayItem.Size = new System.Drawing.Size(198, 22);
            this.recentRepoTrayItem.Text = "최근 작업 프로젝트";
            // 
            // helpTrayItem
            // 
            this.helpTrayItem.Name = "helpTrayItem";
            this.helpTrayItem.Size = new System.Drawing.Size(198, 22);
            this.helpTrayItem.Text = "도움말/문의하기";
            this.helpTrayItem.Click += new System.EventHandler(this.trayItemHelp_Click);
            // 
            // checkUpdateTrayItem
            // 
            this.checkUpdateTrayItem.Name = "checkUpdateTrayItem";
            this.checkUpdateTrayItem.Size = new System.Drawing.Size(198, 22);
            this.checkUpdateTrayItem.Text = "업데이트 확인";
            this.checkUpdateTrayItem.Click += new System.EventHandler(this.trayItemCheckUpdate_Click);
            // 
            // clingrayWebTrayItem
            // 
            this.clingrayWebTrayItem.Name = "clingrayWebTrayItem";
            this.clingrayWebTrayItem.Size = new System.Drawing.Size(198, 22);
            this.clingrayWebTrayItem.Text = "Clingray 웹사이트 가기";
            this.clingrayWebTrayItem.Click += new System.EventHandler(this.trayItemClingrayWeb_Click);
            // 
            // settingsTrayItem
            // 
            this.settingsTrayItem.Name = "settingsTrayItem";
            this.settingsTrayItem.Size = new System.Drawing.Size(198, 22);
            this.settingsTrayItem.Text = "환경설정";
            this.settingsTrayItem.Click += new System.EventHandler(this.trayItemSettings_Click);
            // 
            // exitTrayItem
            // 
            this.exitTrayItem.Name = "exitTrayItem";
            this.exitTrayItem.Size = new System.Drawing.Size(198, 22);
            this.exitTrayItem.Text = "Clingray 종료";
            this.exitTrayItem.Click += new System.EventHandler(this.trayItemExit_Click);
            // 
            // frmFrame
            // 
            this.ClientSize = new System.Drawing.Size(709, 545);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "frmFrame";
            this.Text = "frmFrame";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmFrame_FormClosing);
            this.Shown += new System.EventHandler(this.frmFrame_Shown);
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem currentRepoTrayItem;
        private System.Windows.Forms.ToolStripMenuItem commitTrayItem;
        private System.Windows.Forms.ToolStripMenuItem openTrayItem;
        private System.Windows.Forms.ToolStripMenuItem createRepoTrayItem;
        private System.Windows.Forms.ToolStripMenuItem recentRepoTrayItem;
        private System.Windows.Forms.ToolStripMenuItem helpTrayItem;
        private System.Windows.Forms.ToolStripMenuItem checkUpdateTrayItem;
        private System.Windows.Forms.ToolStripMenuItem clingrayWebTrayItem;
        private System.Windows.Forms.ToolStripMenuItem settingsTrayItem;
        private System.Windows.Forms.ToolStripMenuItem exitTrayItem;

    }
}