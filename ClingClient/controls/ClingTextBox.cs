using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ClingClient
{
    public partial class ClingTextBox : ClingControlBase
    {
        TextBox txtBox;
        Label lblTip;

        public ClingTextBox(string tipText)
        {
            InitializeComponent();
            this.Height = 26;

            lblTip = new Label();
            lblTip.Location = new Point(11, 8);
            lblTip.ForeColor = Color.FromArgb(0xae, 0xae, 0xae);
            lblTip.Text = tipText;
            lblTip.Click += new EventHandler(lblTip_Click);

            txtBox = new TextBox();
            txtBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            txtBox.Location = new Point(10, 8);
            txtBox.BackColor = Color.FromArgb(245, 245, 245);
            txtBox.ForeColor = Color.Black;


            this.Controls.Add(txtBox);
            this.Controls.Add(lblTip);

            lblTip.BringToFront();

            this.SizeChanged += new EventHandler(ClingTextBox_SizeChanged);
            this.txtBox.Click += new EventHandler(ClingTextBox_Click);
            this.txtBox.LostFocus += new EventHandler(txtBox_LostFocus);
            this.txtBox.KeyPress += new KeyPressEventHandler(txtBox_KeyPress);
            this.txtBox.GotFocus += new EventHandler(txtBox_GotFocus);
        }

        void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //Console.WriteLine("txtBox_KeyPress");
            lblTip.Visible = false;            
        }

        void txtBox_GotFocus(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtBox.Text))
            {
                lblTip.Visible = false;
            }
        }

        public void setPasswordMode(bool isPassword)
        {
            if (isPassword) txtBox.PasswordChar = '*';
            else txtBox.PasswordChar = '\0';
        }

        public string getText()
        {
            return txtBox.Text.Trim();
        }

        public void setText(string text)
        {
            lblTip.Visible = false;
            txtBox.Text = text;
        }

        void txtBox_LostFocus(object sender, EventArgs e)
        {
            if (txtBox.Text.Trim().Length == 0)
            {
                lblTip.Visible = true;
                lblTip.BringToFront();
            }
        }

        void lblTip_Click(object sender, EventArgs e)
        {
            txtBox.Focus();
            ClingTextBox_Click(sender, e);
        }

        void ClingTextBox_Click(object sender, EventArgs e)
        {
            lblTip.Visible = false;
        }

        void makeBgImage()
        {
            Graphics g = getDC();

            Bitmap bmpLeft = Properties.Resources.bg_inputbox_26_l;
            Bitmap bmpRight = Properties.Resources.bg_inputbox_26_r;
            Bitmap bmpMiddle = Properties.Resources.bg_inputbox_26_m;

            g.DrawImage(bmpLeft, 0, 0);
            g.DrawImage(bmpRight, this.Width-bmpRight.Width, 0);

            using (TextureBrush brush = new TextureBrush(bmpMiddle, WrapMode.Tile))
            {
                g.FillRectangle(brush, bmpLeft.Width, 0, this.Width - bmpLeft.Width - bmpRight.Width, this.Height);
            }
        }

        void ClingTextBox_SizeChanged(object sender, EventArgs e)
        {
            //txtBox.MinimumSize = this.Size;
            txtBox.Width = this.Width-20;
            lblTip.Width = txtBox.Width;

            makeBgImage();
            this.Invalidate();
        }
    }
}
