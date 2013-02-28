using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ClingClient.controls {
    public partial class ShadowLabel : Label {
        private bool _shadowEnabled = true;
        private float _xOffsetShadow = 0;
        private float _yOffsetShadow = -1;
        private Color _shadowColor = Color.Black;

        //protected override void WndProc(ref Message m)
        //{
        //    const int WM_NCHITTEST = 0x0084;
        //    const int HTTRANSPARENT = (-1);

        //    if (m.Msg == WM_NCHITTEST)
        //    {
        //        m.Result = (IntPtr)HTTRANSPARENT;
        //    }
        //    else
        //    {
        //        base.WndProc(ref m);
        //    }
        //}

        public ShadowLabel() {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            string paintedText = this.Text;
            if (AutoEllipsis && e.Graphics.MeasureString(paintedText, this.Font).Width > this.Width)
            {
                float newWidth;
                do
                {
                    paintedText = paintedText.Substring(0, paintedText.Length - 1);
                    newWidth = e.Graphics.MeasureString(paintedText + "...", this.Font).Width;
                } while (newWidth > this.Width);

                paintedText += "...";
            }

            if (shadowEnabled) {
                e.Graphics.DrawString(paintedText, this.Font, new SolidBrush(_shadowColor),
                    _xOffsetShadow < 0 ? 0 : _xOffsetShadow, 
                    _yOffsetShadow < 0 ? 0 : _yOffsetShadow, 
                    StringFormat.GenericDefault);
            }

            e.Graphics.DrawString(paintedText, this.Font, new SolidBrush(this.ForeColor), 
                _xOffsetShadow < 0 ? -_xOffsetShadow : 0, 
                _yOffsetShadow < 0 ? -_yOffsetShadow : 0, 
                StringFormat.GenericDefault);   
        }

        [Category("Shadow"),
        Description("Set to true to drop shadow."),
        DefaultValue(true)]
        public bool shadowEnabled {
            get { return _shadowEnabled; }
            set { _shadowEnabled = value; Invalidate(); }
        }

        [Category("Shadow"),
        Description("Set to true to drop shadow."),
        DefaultValue(typeof(System.Drawing.Color), "Color.Black")]
        public Color shadowColor {
            get { return _shadowColor; }
            set { _shadowColor = value; Invalidate(); }
        }

        [Category("Shadow"),
        Description("Set x offset of shadow."),
        DefaultValue(-1)]
        public float xOffsetShadow {
            get { return _xOffsetShadow; }
            set { _xOffsetShadow = value; Invalidate(); }
        }

        [Category("Shadow"),
        Description("Set y offset of shadow."),
        DefaultValue(0)]
        public float yOffsetShadow {
            get { return _yOffsetShadow; }
            set { _yOffsetShadow = value; Invalidate(); }
        }
    }
}
