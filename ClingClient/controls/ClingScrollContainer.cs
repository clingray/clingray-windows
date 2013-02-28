using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClingClient
{
    public partial class ClingScrollContainer : UserControl
    {
        double _curScrollRatio;
        public double curScrollRatio 
        {
            get
            {
                return _curScrollRatio;
            }
        }

        public int getContentsWidth
        {
            get
            {
                return container.Width;
            }
        }

        ClingScrollBar scrollBar;
        ClingPanel container;
        int scrollLeftMargin, scrollRightMargin;

        public ClingScrollContainer(int scrollLeftMargin, int scrollRightMargin)
        {
            InitializeComponent();

            this.scrollLeftMargin = scrollLeftMargin;
            this.scrollRightMargin = scrollRightMargin;

            container = new ClingPanel();
            container.Resize += new EventHandler(realPanel_Resize);

            scrollBar = new ClingScrollBar();

            this.MouseDown += new MouseEventHandler(scrollBar_MouseDown);
            this.MouseMove += new MouseEventHandler(scrollBar_MouseMove);
            this.MouseUp += new MouseEventHandler(scrollBar_MouseUp);
            this.Resize += new EventHandler(ClingContainer_Resize);
            this.MouseWheel += new MouseEventHandler(ClingScrollContainer_MouseWheel);

            this.Controls.Add(container);
            this.Controls.Add(scrollBar);
        }

        void ClingScrollContainer_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("ClingScrollContainer_MouseWheel:" + e.Delta);
            double sensitivity = 0.25;
            scroll(curScrollRatio + sensitivity * ((e.Delta > 0) ? -1 : 1));
        }

        public void scroll(double ratio)
        {
            if ((ratio < 0) || (ratio > 1)) return;

            int overHeight = container.Height - this.Height;

            if (overHeight <= 0)
            {
                //스크롤 될 필요 없는 상태므로, 무조건 젤위로 스크롤
                container.Top = 0;
                _curScrollRatio = 0;

                scrollBar.Visible = false;
            }
            else
            {
                container.Top = (int)(overHeight * ratio) * (-1);
                _curScrollRatio = ratio;

                scrollBar.Visible = true;

                //스크롤바의 세로 길이 결정
                int willHeight = this.Height - overHeight;
                if (scrollBar.minHeight > willHeight) willHeight = scrollBar.minHeight;  //스크롤바가 너무 작아지지 않도록 최소값 체크
                scrollBar.Height = willHeight;

                //스크롤바 TOP 결정
                scrollBar.Top = (int)((this.Height - scrollBar.Height) * ratio);
            }

            scrollBar.Invalidate();
        }

        //컨트롤들 품고 있는 진짜 패널 크기가 변경됐을때!
        void realPanel_Resize(object sender, EventArgs e)
        {
            //Console.WriteLine("realPanel_Resize:" + container.Size);
            scroll(_curScrollRatio);
        }

        void scrollBar_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDowned = false;
        }

        void scrollBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDowned)
            {
                int willTop = scrollBar.Top + (e.Location.Y - prePoint.Y);

                if ((willTop >= 0) && ((willTop + scrollBar.Height) <= this.Height))
                {
                    double total_height = this.Height - scrollBar.Height;
                    scroll(willTop / total_height);
                }
                else
                {
                    //Console.WriteLine("over willTop:" + willTop);
                }

                prePoint = e.Location;
            }
        }

        bool mouseDowned = false;
        Point prePoint;
        void scrollBar_MouseDown(object sender, MouseEventArgs e)
        {
            prePoint = e.Location;

            if (!scrollBar.Visible) return;     //스크롤바가 보일때만 액션 동작하도록!
            mouseDowned = true;
        }

        void ClingContainer_Resize(object sender, EventArgs e)
        {
            //Console.WriteLine("ClingContainer_Resize:" + this.Size);

            container.Width = this.Width - scrollBar.Width - scrollLeftMargin - scrollRightMargin;
            scrollBar.Location = new Point(this.Width - scrollBar.Width - scrollRightMargin, 0);
        }

        public void clearControl()
        {
            container.Controls.Clear();
        }

        public void removeControl(Control child)
        {
            container.Controls.Remove(child);
        }
        
        public void addControl(Control child)
        {
            container.Controls.Add(child);
        }

        class ClingPanel : UserControl
        {
            public ClingPanel()
            {
                //this.BackColor = Color.Blue;

                this.ControlAdded += new ControlEventHandler(ClingContainer_ControlAdded);
                this.ControlRemoved += new ControlEventHandler(ClingPanel_ControlRemoved);
            }

            void ClingPanel_ControlRemoved(object sender, ControlEventArgs e)
            {
                getMaxBottom();
            }

            void getMaxBottom()
            {
                int maxBottom = 0;

                foreach (Control child in this.Controls)
                {
                    //Console.WriteLine(child);
                    if (child.Bottom > maxBottom)
                    {
                        maxBottom = child.Bottom;
                    }
                }

                this.Height = maxBottom;
            }

            void ClingContainer_ControlAdded(object sender, ControlEventArgs e)
            {
                //Console.WriteLine("ClingContainer_ControlAdded:" + e.Control);
                getMaxBottom();
            }

            //마우스 이벤트들 뒤로 통과되게!
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
        }

        class ClingScrollBar : UserControl
        {
            Bitmap bgTop = null;
            Bitmap bgMiddle = null;
            Bitmap bgBottom = null;

            public int minHeight 
            {
                get
                {
                    return bgTop.Height + bgBottom.Height + 20;
                }
            }

            public ClingScrollBar()
            {
                this.bgTop = ClingClient.Properties.Resources.img_scroll_top;
                this.bgMiddle = ClingClient.Properties.Resources.img_scroll_mid;
                this.bgBottom = ClingClient.Properties.Resources.img_scroll_bot;

                this.Width = bgTop.Width;
                this.Height = minHeight;

                this.Paint += new PaintEventHandler(ClingScrollBar_Paint);
            }

            //마우스 이벤트들 뒤로 통과되게!
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

            void ClingScrollBar_Paint(object sender, PaintEventArgs e)
            {
                Graphics g = e.Graphics;

                g.DrawImage(bgTop, 0, 0);

                for (int y = bgTop.Height; y < (this.Height - bgBottom.Height); y += bgMiddle.Height)
                {
                    g.DrawImage(bgMiddle, 0, y);
                }

                g.DrawImage(bgBottom, 0, this.Height - bgBottom.Height);

            }
        }
    }
}
