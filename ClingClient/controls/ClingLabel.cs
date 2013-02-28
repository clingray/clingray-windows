using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.utilities;

namespace ClingClient
{
    public partial class ClingLabel : ClingControlBase
    {
        public Label label;

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTTRANSPARENT = (-1);

            if (m.Msg == WM_NCHITTEST)
            {
                m.Result = (IntPtr)HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        public ClingLabel(string text)
        {
            InitializeComponent();

            label = new Label();
            label.Text = text;
            label.AutoSize = true;
            label.SizeChanged += new EventHandler(label_SizeChanged);
            
            this.Controls.Add(label);
        }

        void label_SizeChanged(object sender, EventArgs e)
        {
            this.Size = label.Size;
        }
    }
}
