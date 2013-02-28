using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.Properties;

namespace ClingClient
{
    public partial class ClingProgress : ClingControlBase
    {
        public Bitmap bmpBackSingle { get; set; }
        
        public Bitmap bmpBackLeft { get; set; }
        public Bitmap bmpBackRight { get; set; }
        public Bitmap bmpBackMiddle { get; set; }

        public Bitmap bmpFrontLeft { get; set; }
        public Bitmap bmpFrontRight { get; set; }
        public Bitmap bmpFrontMiddle { get; set; }

        double _currentValue;
        public double currentValue 
        {
            get
            {
                return _currentValue;
            }

            set
            {
                _currentValue = value;
                if (_currentValue > 100) _currentValue = 100;
                this.Invalidate();
            }
        }

        public ClingProgress()
        {
            InitializeComponent();
            if (bmpBackSingle != null) {
                this.Height = bmpBackSingle.Height;
                this.Width = bmpBackSingle.Width;
            } else if (bmpBackLeft != null) {
                this.Height = bmpBackLeft.Height;
            }
            
            this.Paint += new PaintEventHandler(ClingProgress_Paint);
        }

        void ClingProgress_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (bmpBackSingle != null) {
                g.DrawImage(bmpBackSingle, 0, 0);
            } else if(bmpBackLeft != null &&
                bmpBackMiddle != null &&
                bmpBackRight != null) {
                g.DrawImage(bmpBackLeft, 0, 0);
                g.DrawImage(bmpBackRight, this.Width - bmpBackRight.Width, 0);
                for (int x = bmpBackLeft.Width; x < (this.Width - bmpBackRight.Width); x += bmpBackMiddle.Width) {
                    g.DrawImage(bmpBackMiddle, x, 0);
                }
            }

            if (bmpFrontLeft != null && 
                bmpFrontMiddle != null &&
                bmpFrontRight != null &&
                currentValue > 0) {
                g.DrawImage(bmpFrontLeft, 0, 0);

                double totalWidth = this.Width - bmpFrontLeft.Width - bmpFrontRight.Width;
                double shouldWidth = (currentValue / 100) * totalWidth;

                for (int x = bmpFrontLeft.Width; x < (this.Width - bmpFrontRight.Width); x += bmpFrontMiddle.Width) {
                    if (x > shouldWidth) {
                        g.DrawImage(bmpFrontRight, x, 0);
                        break;
                    }
                    g.DrawImage(bmpFrontMiddle, x, 0);
                }
            }
        }
    }
}
