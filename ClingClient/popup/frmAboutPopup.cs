using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using ClingClient.Properties;
using System.Resources;
using System.Collections;
using System.Diagnostics;
using ClingClient.utilities;

namespace ClingClient.forms {
    public partial class frmAboutPopup : frmCommonPopup {
        public enum DisplayMode {
            SPLASH,
            ABOUT
        };

        private DisplayMode displayMode = DisplayMode.ABOUT;
        Label lblLink;

        bool isSplashMode() {
            return displayMode == DisplayMode.SPLASH;
        }

        public frmAboutPopup(DisplayMode displayMode) {
            InitializeComponent();
            this.displayMode = displayMode;
            setControls();

            if (isSplashMode())
            {
                showCloseButton = false;
            }
            else
            {
                showCloseButton = true;
            }

            showClingLogo = false;
            showButtonLine = false;

            this.ShowInTaskbar = false;
            this.Size = new Size(294, 367);
            this.Resize += new EventHandler(frmAbout_Resize);
            this.Click += new EventHandler(frmAbout_Click);
        }

        void frmAbout_Resize(object sender, EventArgs e)
        {
            if (!isSplashMode())
            {
                lblLink.Location = new Point(this.Width / 2 - lblLink.Width / 2, this.Height - 100);
            }
        }

        void setControls()
        {
            if (!isSplashMode())
            {
                lblLink = new Label();
                lblLink.Text = "http://www.clingray.com";
                lblLink.Font = new System.Drawing.Font("돋음", 11F, FontStyle.Underline, GraphicsUnit.Pixel);
                lblLink.ForeColor = Color.FromArgb(0x008FD5);
                lblLink.BackColor = Color.FromArgb(239, 239, 239);
                lblLink.AutoSize = true;
                lblLink.Click += new EventHandler(lblLink_Click);
                
                this.Controls.Add(lblLink);
            }
        }

        void lblLink_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.clingray.com");
        }

        protected override void makeBackgroundImage(Graphics g)
        {
            //가재미 로고
            Bitmap bmpLogo = Properties.Resources.img_logo_cling;
            drawImage(g, bmpLogo, (this.Width / 2F - bmpLogo.Width / 2F), 44);

            //clingray 글자
            Bitmap bmpClingray = Properties.Resources.logo_cling_114;
            drawImage(g, bmpClingray, (this.Width / 2F - bmpClingray.Width / 2F), 44 + 15 + bmpLogo.Height);

            //version
            Bitmap versionBitmap = getVersionImage();
            drawImage(g, versionBitmap, (this.Width / 2F - versionBitmap.Width / 2F), 44 + 15 + bmpLogo.Height + 7 + bmpClingray.Height);

            //copyright
            Bitmap bmpCopyright = Properties.Resources.txt_copyright;
            drawImage(g, bmpCopyright, (this.Width / 2F - bmpCopyright.Width / 2F), this.Height - 21 - bmpCopyright.Height);
        }

        const int VERSION_MARGIN = 3, NUMBER_MARGIN = 1;
        private Bitmap getVersionImage()
        {
            Hashtable fontTable = new Hashtable();
            fontTable.Add('0', Resources.txt_login_version_0);
            fontTable.Add('1', Resources.txt_login_version_1);
            fontTable.Add('2', Resources.txt_login_version_2);
            fontTable.Add('3', Resources.txt_login_version_3);
            fontTable.Add('4', Resources.txt_login_version_4);
            fontTable.Add('5', Resources.txt_login_version_5);
            fontTable.Add('6', Resources.txt_login_version_6);
            fontTable.Add('7', Resources.txt_login_version_7);
            fontTable.Add('8', Resources.txt_login_version_8);
            fontTable.Add('9', Resources.txt_login_version_9);
            fontTable.Add('.', Resources.txt_login_version_dot);
            Bitmap bmpVersion = Properties.Resources.txt_login_version;

            string versionStr = ApplicationUtils.getApplicationVersion();

            // 너비 계산.
            int width = bmpVersion.Width + VERSION_MARGIN;
            for (int i = 0; i < versionStr.Length; i++)
            {
                char ch = versionStr.ElementAt(i);
                Bitmap font = (Bitmap)fontTable[ch];
                width += font.Width + NUMBER_MARGIN;
            }

            // 그리기.
            Bitmap versionBitmap = new Bitmap(width, bmpVersion.Height);
            Graphics graphics = Graphics.FromImage(versionBitmap);

            float x = 0;
            drawImage(graphics, bmpVersion, x, 0); x += bmpVersion.Width + VERSION_MARGIN;
            for (int i = 0; i < versionStr.Length; i++)
            {
                char ch = versionStr.ElementAt(i);
                Bitmap font = (Bitmap)fontTable[ch];
                graphics.DrawImage(font, x, 0);
                x += font.Width + NUMBER_MARGIN;
            }

            return versionBitmap;
        }

        private void frmAbout_Click(object sender, EventArgs e) {
            if (!isSplashMode()) {
                Close();
            }
        }
    }
}
