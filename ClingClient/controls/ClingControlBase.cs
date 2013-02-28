using System;
using System.Drawing;
using System.Windows.Forms;
using ClingClient.forms;
using ClingClient.common;

namespace ClingClient
{
    public partial class ClingControlBase : UserControl
    {
        protected Bitmap paintBuffer;

        public ClingControlBase()
        {
            InitializeComponent();

            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;

            //배경 투명 처리
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;

            //더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();


            //this.BackColor = Color.Red;

            this.Paint += new PaintEventHandler(ClingControlBase_Paint);
        }

        void ClingControlBase_Paint(object sender, PaintEventArgs e)
        {
            if (paintBuffer != null)
            {
                drawImage(e.Graphics, paintBuffer, 0, 0);
            }
        }

        protected void drawImage(Graphics g, Bitmap bmp, float x, float y)
        {
            g.DrawImage(bmp, x, y, bmp.Width, bmp.Height);
        }

        protected Graphics getDC()
        {
            // Avoid crash on minimize.
            int width = this.Width > 0 ? this.Width : 1;
            int height = this.Height > 0 ? this.Height : 1;
            
            if (paintBuffer != null) paintBuffer.Dispose();
            paintBuffer = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(paintBuffer);

            return g;
        }

        #region 상대위치 지정 관련 

        public void moveParentTopLeft(int xMargin, int yMargin)
        {
            ControlLocator.moveParentTopLeft(this, xMargin, yMargin);
        }

        public void moveParentBottomLeft(Control parent, int xMargin, int yMargin)
        {
            ControlLocator.moveParentBottomLeft(this, parent, xMargin, yMargin);
        }

        public void moveParentBottomRight(Control parent, int xMargin, int yMargin)
        {
            ControlLocator.moveParentBottomRight(this, parent, xMargin, yMargin);
        }

        public void moveParentTopRight(Control parent, int xMargin, int yMargin)
        {
            ControlLocator.moveParentTopRight(this, parent, xMargin, yMargin);
        }
        
        public void moveParentRight(Control parent, int xMargin)
        {
            ControlLocator.moveParentRight(this, parent, xMargin);
        }

        public void moveBelow(Control preControl, int yMargin)
        {
            ControlLocator.moveBelow(this, preControl, yMargin);
        }

        public void moveRight(Control preControl, int xMargin)
        {
            ControlLocator.moveRight(this, preControl, xMargin);
        }

        public void moveLeft(Control preControl, int xMargin)
        {
            ControlLocator.moveLeft(this, preControl, xMargin);
        }

        #endregion
    }
}
