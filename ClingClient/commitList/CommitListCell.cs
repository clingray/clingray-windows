using System;
using System.Windows.Forms;
using ClingClient.Properties;
using ClingClientEngine;
using System.Reflection;
using System.Resources;
using System.Drawing;
using ClingClient.common;
using ClingClient.utilities;

namespace ClingClient.controls {
    public partial class CommitListCell : ClingControlBase {
        public Commit commitInfo { get; set; }
        public bool isCurrentVersion { get; set; }
        private bool isReadOnly;

        NinepatchDrawer bgDrawer = new NinepatchDrawer();
        public const int CURRENT_VERSION_WIDTH = 652, ANOTHER_VERSION_WIDTH = 639;

        public CommitListCell(Commit commitInfo, bool isReadOnly) {
            bgDrawer.topLeftBmp = Resources.bg_listbar_top_l;
            bgDrawer.topCenterBmp = Resources.bg_listbar_top_m;
            bgDrawer.topRightBmp = Resources.bg_listbar_top_r;
            bgDrawer.midLeftBmp = Resources.bg_listbar_mid_l;
            bgDrawer.midCenterBmp = Resources.bg_listbar_mid_m;
            bgDrawer.midRightBmp = Resources.bg_listbar_mid_r;
            bgDrawer.botLeftBmp = Resources.bg_listbar_bot_l;
            bgDrawer.botCenterBmp = Resources.bg_listbar_bot_m;
            bgDrawer.botRightBmp = Resources.bg_listbar_bot_r;

            InitializeComponent();
            
            this.commitInfo = commitInfo;
            this.isReadOnly = isReadOnly;
        }

        private void CommitListCell_Paint(object sender, PaintEventArgs e) {
            bgDrawer.drawNinepatch(this.Size, e.Graphics);
        }

        private void CommitListCell_Load(object sender, EventArgs e) {
            DateTime commitDate = commitInfo.commitDate;

            viewFilesButton.Visible = isCurrentVersion && !isReadOnly;
            checkoutButton.Visible = !isReadOnly;

            // Month
            string monthBmpName = string.Format("img_date_mm_{0:d2}", commitDate.Month);
            Bitmap monthBmp = (Bitmap)Resources.ResourceManager.GetObject(monthBmpName, Resources.Culture);
            picMonth.Image = monthBmp;

            // Date
            string dayBmpName = string.Format("img_date_dd_{0:d2}", commitDate.Day);
            Bitmap dayBmp = (Bitmap)Resources.ResourceManager.GetObject(dayBmpName, Resources.Culture);
            picDay.Image = dayBmp;

            // Timestamp
            timestampLabel.Text = commitDate.ToString("yyyy-MM-dd HH:mm:ss");

            // Comment
            commitMessage.Text = commitInfo.comment;

            ToolTipHelper.add(commitMessage, commitInfo.comment);

            if (isCurrentVersion) {
                currentIndicator.Show();
            } else {
                panel.Left -= CURRENT_VERSION_WIDTH - ANOTHER_VERSION_WIDTH;
                this.Left += CURRENT_VERSION_WIDTH - ANOTHER_VERSION_WIDTH;
                this.Width -= CURRENT_VERSION_WIDTH - ANOTHER_VERSION_WIDTH;
            }

            if (isCurrentVersion)
            {
                commitMessage.Width = viewFilesButton.Left - 20 - (commitMessage.Left + panel.Left);
            }
            else if (!isReadOnly)
            {
                commitMessage.Width = checkoutButton.Left - 20 - (commitMessage.Left + panel.Left);
            }
            else
            {
                commitMessage.Width = checkoutButton.Right - 20 - (commitMessage.Left + panel.Left);
            }
        }
    }
}
