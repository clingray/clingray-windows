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
    public partial class ClingButton : ClingControlBase, IButtonControl
    {
        Bitmap _bmpNormal = null;
        public Bitmap bmpNormal
        {
            get
            {
                return _bmpNormal;
            }
            set
            {
                _bmpNormal = value;
                this.BackgroundImage = value;
                if (value != null) {
                    this.Size = value.Size;
                }
            }
        }

        public Bitmap bmpPress { get; set; }
        public Bitmap bmpIn { get; set; }
        public string text { get; set; }

        public event EventHandler ButtonClick;

        public ClingButton()
        {
            InitializeComponent();

            this.TabStop = false;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ClingButton_MouseDown);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ClingButton_MouseUp);
            this.MouseEnter += new EventHandler(ClingButton_MouseEnter);
            this.MouseLeave += new EventHandler(ClingButton_MouseLeave);
        }

        public void setToolTip(string msg)
        {
            ToolTipHelper.add(this, msg);
        }

        public void setImages(Bitmap normal, Bitmap press, Bitmap hover = null)
        {
            bmpNormal = normal;
            bmpPress = press;

            if (hover != null) bmpIn = hover;
            else bmpIn = normal;
        }

        void ClingButton_MouseEnter(object sender, EventArgs e)
        {
            if (bmpIn != null)
            {
                this.BackgroundImage = bmpIn;
            }
        }

        void ClingButton_MouseLeave(object sender, EventArgs e)
        {
            if (bmpNormal != null)
            {
                this.BackgroundImage = bmpNormal;
            }
        }

        private void ClingButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (bmpPress != null)
            {
                this.BackgroundImage = bmpPress;
            }
        }

        private void ClingButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (bmpNormal != null)
            {
                this.BackgroundImage = bmpNormal;
            }

            if (ButtonClick != null)
            {
                this.ButtonClick(sender, e);
            }
        }

        private void ClingButton_Paint(object sender, PaintEventArgs e) {
            if (text != null) {
                // Create a StringFormat object with the each line of text, and the block 
                // of text centered on the page.
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;

                RectangleF rect = new RectangleF();
                rect.X = 0;
                rect.Y = 3;
                rect.Width = this.Width;
                rect.Height = this.Height;

                Brush brush = new SolidBrush(this.ForeColor);

                // Draw the text and the surrounding rectangle.
                e.Graphics.DrawString(text, this.Font, brush, rect, stringFormat);
            }
        }

        // 요약:
        //     단추를 클릭할 때 부모 폼에 반환되는 값을 가져오거나 설정합니다.
        //
        // 반환 값:
        //     System.Windows.Forms.DialogResult 값 중 하나입니다.
        public DialogResult DialogResult { get; set; }

        // 요약:
        //     해당 모양과 동작이 적절하게 조정되도록 이 단추가 기본 단추임을 컨트롤에 알립니다.
        //
        // 매개 변수:
        //   value:
        //     컨트롤이 기본 단추로 동작해야 하면 true이고, 그렇지 않으면 false입니다.
        public void NotifyDefault(bool value)
        {
            //Console.WriteLine("NotifyDefault");
        }
        //
        // 요약:
        //     컨트롤에 대해 System.Windows.Forms.Control.Click 이벤트를 생성합니다.
        public void PerformClick()
        {
            //Console.WriteLine("PerformClick");

            if (ButtonClick != null)
            {
                this.ButtonClick(this, null);
            }
        }
    }
}
