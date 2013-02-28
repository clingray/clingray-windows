using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.controls;
using System.Drawing.Imaging;
using ClingClient.Properties;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace ClingClient.forms 
{
    public partial class frmCommonPopup : Form 
    {
        Bitmap paintBuffer;
        protected ClingButton btnClose;

        //오른쪽 상단에 닫기 버튼 표기 여부
        bool _showCloseButton;
        public bool showCloseButton
        {
            get
            {
                return _showCloseButton;
            }
            set
            {
                _showCloseButton = value;
                refreshPopup();
            }
        }

        //왼쪽 상단에 클링 로고 표시할지 말지 여부
        bool _showClingLogo;
        public bool showClingLogo {
            get
            {
                return _showClingLogo;
            }
            set
            {
                _showClingLogo = value;
                refreshPopup();
            }
        }

        //하단 버튼 위에 가로선 그을지 말지
        bool _showButtonLine;
        public bool showButtonLine
        {
            get
            {
                return _showButtonLine;
            }
            set
            {
                _showButtonLine = value;
                refreshPopup();
            }
        }

        NinepatchDrawer bgDrawer = new NinepatchDrawer();

        #region Bitmap properties
        public Bitmap bgTopLeft
        {
            get { return bgDrawer.topLeftBmp; }
            set { bgDrawer.topLeftBmp = value; }
        }
        public Bitmap bgTopCenter
        {
            get { return bgDrawer.topCenterBmp; }
            set { bgDrawer.topCenterBmp = value; }
        }
        public Bitmap bgTopRight
        {
            get { return bgDrawer.topRightBmp; }
            set { bgDrawer.topRightBmp = value; }
        }

        public Bitmap bgMidLeft
        {
            get { return bgDrawer.midLeftBmp; }
            set { bgDrawer.midLeftBmp = value; }
        }
        public Bitmap bgMidCenter
        {
            get { return bgDrawer.midCenterBmp; }
            set { bgDrawer.midCenterBmp = value; }
        }
        public Bitmap bgMidRight
        {
            get { return bgDrawer.midRightBmp; }
            set { bgDrawer.midRightBmp = value; }
        }

        public Bitmap bgBotLeft
        {
            get { return bgDrawer.botLeftBmp; }
            set { bgDrawer.botLeftBmp = value; }
        }
        public Bitmap bgBotCenter
        {
            get { return bgDrawer.botCenterBmp; }
            set { bgDrawer.botCenterBmp = value; }
        }
        public Bitmap bgBotRight
        {
            get { return bgDrawer.botRightBmp; }
            set { bgDrawer.botRightBmp = value; }
        }
        #endregion

        public frmCommonPopup()
        {
            InitializeComponent();

            bgTopLeft = Resources.bg_popup_notit_l;
            bgTopCenter = Resources.bg_popup_notit_m;
            bgTopRight = Resources.bg_popup_notit_r;

            bgMidLeft = Resources.bg_popup_mid_l;
            bgMidCenter = Resources.bg_popup_mid_m;
            bgMidRight = Resources.bg_popup_mid_r;

            bgBotLeft = Resources.bg_popup_bot_l;
            bgBotCenter = Resources.bg_popup_bot_m;
            bgBotRight = Resources.bg_popup_bot_r;

            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;

            this.Icon = Properties.Resources.cling;
            this.Text = "Clingray";

            //더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();

            this.TransparencyKey = Color.Fuchsia;
            this.BackColor = Color.Fuchsia;

            this.Load += new EventHandler(frmPopupFrame_Load);
            this.Paint += new PaintEventHandler(frmPopupFrame_Paint);
            this.MouseDown += new MouseEventHandler(frmPopupFrame_MouseDown);
            this.MouseMove += new MouseEventHandler(frmPopupFrame_MouseMove);
            this.MouseUp += new MouseEventHandler(frmPopupFrame_MouseUp);
            this.Resize += new EventHandler(frmCommonPopup_Resize);

            refreshPopup();
        }

        void frmCommonPopup_Resize(object sender, EventArgs e)
        {
            //owner 가운데로 가도록 수동 조절!
            if (Program.mainframe != null)
            {
                this.Left = Program.mainframe.Left + (int)(Program.mainframe.Width / 2F - base.Width / 2F);
                this.Top = Program.mainframe.Top + (int)(Program.mainframe.Height / 2F - base.Height / 2F);
            }

            refreshPopup();
        }

        protected void refreshPopup()
        {
            _makeBackgroundImage();
            this.Invalidate();
        }

        protected void drawImage(Graphics g, Bitmap bmp, float x, float y)
        {
            g.DrawImage(bmp, x, y, bmp.Width, bmp.Height);
        }

        Graphics getDC()
        {
            if (paintBuffer != null) paintBuffer.Dispose();
            paintBuffer = new Bitmap(base.Width, base.Height);
            Graphics g = Graphics.FromImage(paintBuffer);

            return g;
        }

        void frmPopupFrame_Load(object sender, EventArgs e)
        {
            //닫기 버튼
            btnClose = new ClingButton();
            btnClose.setImages(Properties.Resources.btn_apptop_close02_normal,
                Properties.Resources.btn_apptop_close02_press,
                Properties.Resources.btn_apptop_close02_hover);
            btnClose.moveParentTopRight(this, 5, 4);
            btnClose.ButtonClick += new EventHandler(btnClose_ButtonClick);
            btnClose.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            if(showCloseButton) this.Controls.Add(btnClose);
        }

        protected void btnClose_ButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        void frmPopupFrame_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDownPoint = Point.Empty;
        }

        void frmPopupFrame_MouseMove(object sender, MouseEventArgs e)
        {
            if (!mouseDownPoint.IsEmpty)
            {
                Point newPoint = e.Location;

                //폼 위치이동
                int gapX = (newPoint.X - mouseDownPoint.X);
                int gapY = (newPoint.Y - mouseDownPoint.Y);

                this.Left += gapX;
                this.Top += gapY;

            }
        }

        Point mouseDownPoint = Point.Empty;
        void frmPopupFrame_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDownPoint = e.Location;
        }

        void frmPopupFrame_Paint(object sender, PaintEventArgs e)
        {
            if (paintBuffer != null)
            {
                e.Graphics.DrawImage(paintBuffer, 0, 0);
            } 
        }

        //상속받은 놈들이 구현할것!
        protected virtual void makeBackgroundImage(Graphics g) {
        }

        void _makeBackgroundImage()
        {
            Graphics g = getDC();

            //사각형 테두리 버젼
            {
                //그림자
                g.FillRectangle(new SolidBrush(Color.FromArgb(shadowColor, shadowColor, shadowColor)), shadowGap, shadowGap, base.Width, base.Height);

                //배경색
                g.FillRectangle(new SolidBrush(Color.FromArgb(239, 239, 239)), 1, 1, base.Width - shadowGap, base.Height - shadowGap);

                //테두리
                g.DrawRectangle(new Pen(Color.FromArgb(100, 100, 100)), 1, 1, base.Width - shadowGap, base.Height - shadowGap);
            }
            

            //bgDrawer.drawNinepatch(this.Size, g);

            Bitmap bmpEx = Properties.Resources.ico_popup_info;
            Bitmap bmpClingLogo = Properties.Resources.logo_cling;


            //cling logo
            if (showClingLogo) g.DrawImage(bmpClingLogo, 8, 8);

            if (showButtonLine)
            {
                //버튼위에 가로 줄
                float bottomLineTop = getAboveButtonLineTop();
                g.DrawLine(new Pen(Color.FromArgb(0xE0, 0xE0, 0xE0), 1F), 2, bottomLineTop, Width, bottomLineTop);
            }

            //custom drawing
            makeBackgroundImage(g);
        }

        //하단 버튼 왼쪽에 설명부분 좌표 리턴
        protected Point getButtonCommentLocation()
        {
            Point pt = new Point(26, Height - 37);
            return pt;
        }


        //좌측상단 타이틀 제목 좌표 리턴
        protected Point getContentTopLocation()
        {
            Point pt = new Point(Program.POPUP_BOTTOM_BUTTON_H_MARGIN, 28 + 25);
            return pt;
        }

        //하단 버튼들 위에 뿌려져야 하는 가로선 top 리턴
        public int getAboveButtonLineTop()
        {
            return base.Height - (15 * 2) - 31 - shadowGap;   //버튼height31, 여백15씩
        }

        //하단 버튼들 뿌려져야 하는 top 리턴
        public int getButtonTop()
        {
            return base.Height - 15 - 31 - shadowGap;   //버튼height31, 여백15씩
        }

        public static int shadowGap = 3;
        public static int shadowColor = 100;

        public new int Width
        {
            get
            {
                return base.Width - shadowGap;
            }

            set
            {
                base.Width = value;
            }
        }

        public new int Height
        {
            get
            {
                return base.Height - shadowGap;
            }

            set
            {
                base.Height = value;
            }
        }

        public new Size Size
        {
            get
            {
                return new Size(Width, Height);
            }

            set
            {
                Width = value.Width + shadowGap;
                Height = value.Height + shadowGap;
            }
        }

    }
}
