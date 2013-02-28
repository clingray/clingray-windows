using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.Properties;
using ClingClient.utilities;

namespace ClingClient.commitList
{
    public partial class ProjectNamePanel : UserControl
    {
        private const int EDIT_BUTTON_MARGIN = 5;

        private bool _shouldShowEditButton = false;
        private bool shouldShowEditButton
        {
            get
            {
                return _shouldShowEditButton;
            }

            set
            {
                _shouldShowEditButton = value;
                Invalidate();
            }
        }

        private Rectangle editButtonRect = new Rectangle(0, 0, Resources.btn_tit_edit.Width, Resources.btn_tit_edit.Height);

        [Category("Text"),
        Description("Set text to display."),
        DefaultValue(typeof(string), "")]
        public override string Text
        {
            get
            {
                return nameLabel.Text;
            }
            set
            {
                if (!nameLabel.IsDisposed)
                {
                    Graphics g = Graphics.FromHwnd(nameLabel.Handle);
                    SizeF measuredSize = g.MeasureString(value, nameLabel.Font);
                    nameLabel.Width = Math.Min((int)measuredSize.Width + nameLabel.Margin.Size.Width, nameLabel.MaximumSize.Width);
                    nameLabel.Text = value;


                    ToolTipHelper.add(nameLabel, value);
                }
            }
        }

        public ProjectNamePanel()
        {
            InitializeComponent();
        }

        private void nameLabel_SizeChanged(object sender, EventArgs e)
        {
            editButtonRect.X = nameLabel.Right + EDIT_BUTTON_MARGIN;
        }

        private void nameEditBox_VisibleChanged(object sender, EventArgs e)
        {
            if (nameEditBox.Visible)
            {
                nameEditBox.Text = nameLabel.Text;
            }
        }

        private void ProjectNamePanel_SizeChanged(object sender, EventArgs e)
        {
            nameLabel.Top = (this.Height - nameLabel.Height) / 2;
            nameLabel.MaximumSize = new Size(this.Width - editButtonRect.Width - EDIT_BUTTON_MARGIN, nameLabel.Height);

            editButtonRect.Y = nameLabel.Top + 4;

            nameEditBox.Left = 0;
            nameEditBox.Top = (this.Height - nameEditBox.Height) / 2;
            nameEditBox.Width = this.Width;
        }

        private void nameEditBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                setEditMode(false);
                e.SuppressKeyPress = true;
            }
        }

        private void setEditMode(bool isEditMode)
        {
            nameEditBox.Visible = isEditMode;
            if (isEditMode)
            {
                nameEditBox.Focus();
            }
            else
            {
                if(nameEditBox.Text.Length > 0 && !string.IsNullOrWhiteSpace(nameEditBox.Text)) 
                {
                    Text = nameEditBox.Text;
                }
            }

            nameLabel.Visible = !isEditMode;
            shouldShowEditButton = nameLabel.Visible;
        }

        private void ProjectNamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!shouldShowEditButton)
            {
                shouldShowEditButton = true;
            }
        }

        private void nameEditBox_Leave(object sender, EventArgs e)
        {
            setEditMode(false);
        }

        private void ProjectNamePanel_MouseLeave(object sender, EventArgs e)
        {
            shouldShowEditButton = false;
        }

        private void nameLabel_MouseEnter(object sender, EventArgs e)
        {
            shouldShowEditButton = true;
        }

        private void ProjectNamePanel_Paint(object sender, PaintEventArgs e)
        {
            if (shouldShowEditButton)
            {
                e.Graphics.DrawImage(Resources.btn_tit_edit, editButtonRect);
            }
        }

        private void ProjectNamePanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (editButtonRect.Contains(e.Location))
            {
                setEditMode(true);
            }
        }
    }
}
