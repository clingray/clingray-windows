using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ClingClientEngine;
using System.Runtime.InteropServices;
using ClingClient.popup;
using ClingClient.Properties;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace ClingClient.forms
{
    public partial class frmAlertPopup : frmCommonPopup
    {
        Bitmap topIcon;
        Bitmap bottomImage;

        string alertTitle;
        string alertMessage;
        Size popupSize;
        int messageLeft = 30;        //메시지 표시할 왼쪽 좌표
        IAlertButtonSet buttonProvider;

        public string link_url { get; set; }

        public frmAlertPopup(Size popupSize, Bitmap topIcon, string alertTitle, string alertMessage, IAlertButtonSet buttonProvider, Bitmap bottomImage)
        {
            this.popupSize = popupSize;
            this.topIcon = topIcon;
            this.alertTitle = alertTitle;
            this.alertMessage = alertMessage;
            this.buttonProvider = buttonProvider;
            this.bottomImage = bottomImage;            

            InitializeComponent();
            setControls();

            showCloseButton = true;
            showButtonLine = true;

            this.Size = popupSize;
            this.Resize += new EventHandler(frmAddProjectPopup_Resize);
            this.Load += new EventHandler(frmAddProjectPopup_Load);

            this.MouseMove += new MouseEventHandler(frmAlertPopup_MouseMove);
            this.MouseClick += new MouseEventHandler(frmAlertPopup_MouseClick);
        }

        void frmAlertPopup_MouseClick(object sender, MouseEventArgs e)
        {
            if (is_link_area(e.Location))
            {
                Process.Start(link_url);
            }
        }

        void frmAlertPopup_MouseMove(object sender, MouseEventArgs e)
        {
            if (is_link_area(e.Location)) Cursor.Current = Cursors.Hand;
        }

        bool is_link_area(Point pt)
        {
            foreach (RectangleF rect in link_rect_list)
            {
                if (rect.Contains(pt)) return true;
            }

            return false;
        }

        void frmAddProjectPopup_Load(object sender, EventArgs e)
        {
            
        }

        void setControls()
        {
            buttonProvider.addButtons(this);
        }

        void frmAddProjectPopup_Resize(object sender, EventArgs e)
        {
            buttonProvider.arrangeButtons(this);
        }

        List<string> WrapText(string text, Graphics g, float width, Font font)
        {
            List<string> wrappedLines = new List<string>();
            int startPos = 0;
            int length = 1;
            string preTmpText = null;

            while ((startPos + length) <= text.Length)
            {
                string tmpText = text.Substring(startPos, length);
                SizeF size = g.MeasureString(tmpText, font);

                if (size.Width > width)
                {
                    wrappedLines.Add(preTmpText);

                    preTmpText = null;
                    startPos += (length-1);
                    length = 1;
                }
                else
                {
                    length++;
                }

                preTmpText = tmpText;
            }

            if(preTmpText!=null) wrappedLines.Add(preTmpText);

            return wrappedLines;
        }

        void drawShadowString(Graphics g, string text, Font font, Brush brush, float x, float y)
        {
            g.DrawString(text, font, new SolidBrush(Color.FromArgb(0xf8, 0xf8, 0xf8)), x, y - 1);
            g.DrawString(text, font, brush, x, y);
        }


        enum PhraseType
        {
            PLAIN,
            EMPHASIS,
            WARNING,
            LINK
        };

        public const char TEXT_EMPHASIS_CHAR = '$';
        public const char TEXT_WARNING_CHAR = '^';
        public const char TEXT_LINK_CHAR = '|';

        protected override void makeBackgroundImage(Graphics g)
        {
            float curY = 37;
            link_rect_list = new List<RectangleF>();

            //top 아이콘
            if (topIcon != null)
            {
                g.DrawImage(topIcon, this.Width / 2F - topIcon.Width / 2F, curY);
                curY += (topIcon.Height + 13);
            }


            //타이틀
            if (alertTitle != null) 
            {
                Font fontCmt1 = new System.Drawing.Font("돋음", 11F, FontStyle.Bold, GraphicsUnit.Pixel);

                //줄 나눔 처리
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                SizeF size = g.MeasureString(alertTitle, fontCmt1, this.Width, sf);
                g.DrawString(alertTitle, fontCmt1, new SolidBrush(Color.Black), new RectangleF(0, curY, this.Width, size.Height), sf);

                curY += (23 + size.Height);
            }


            Bitmap bmpPoint = Resources.ico_dot;
            //메시지
            if( !string.IsNullOrWhiteSpace(alertMessage) )
            {
                Font fontCmt1 = new System.Drawing.Font("돋음", 11F, FontStyle.Regular, GraphicsUnit.Pixel);

                //엔터별로 나누기
                string[] msgs = alertMessage.Split(new char[] { '\n' });
                int dotAndTextMargin = 5;   //쩜과 텍스트 사이 가로 여백

                foreach (String msg in msgs)
                {
                    //라인 왼쪽에 쩜.
                    g.DrawImage(bmpPoint, messageLeft, curY + dotAndTextMargin);

                    //라인 전체 width
                    float lineWidth = this.Width - messageLeft * 2 - bmpPoint.Width - dotAndTextMargin;

                    //라인을 width에 맞게 wordwrap 처리
                    List<string> lines = WrapText(msg, g, lineWidth, fontCmt1);

                    PhraseType currentPhraseType = PhraseType.PLAIN;

                    //실제 라인별로 처리
                    foreach (string line in lines)
                    {
                        float curX = messageLeft + bmpPoint.Width + dotAndTextMargin;
                        SizeF lineSize = g.MeasureString(line, fontCmt1);

                        string subString = string.Empty;

                        for(int i = 0; i < line.Length; i++)
                        {
                            // 블록 타입이 바뀔때마다 이전 블록들을 그려준다.
                            if (
                                (line[i] == TEXT_EMPHASIS_CHAR) || 
                                (line[i] == TEXT_WARNING_CHAR) ||
                                (line[i] == TEXT_LINK_CHAR) 
                            )
                            {
                                drawPhraseBlock(currentPhraseType, g, ref curX, curY, subString);

                                if (line[i] == TEXT_EMPHASIS_CHAR)
                                {
                                    currentPhraseType = (currentPhraseType == PhraseType.EMPHASIS) ? PhraseType.PLAIN : PhraseType.EMPHASIS;
                                }
                                else if (line[i] == TEXT_WARNING_CHAR)
                                {
                                    currentPhraseType = (currentPhraseType == PhraseType.WARNING) ? PhraseType.PLAIN : PhraseType.WARNING;
                                }
                                else if (line[i] == TEXT_LINK_CHAR)
                                {
                                    currentPhraseType = (currentPhraseType == PhraseType.LINK) ? PhraseType.PLAIN : PhraseType.LINK;
                                }


                                subString = string.Empty;
                                continue;
                            }

                            subString += line[i];
                        }
                        // 잔여 블록을 그린다.
                        if (!string.IsNullOrWhiteSpace(subString))
                        {
                            drawPhraseBlock(currentPhraseType, g, ref curX, curY, subString);
                        }

                        curY += (lineSize.Height+3);
                    }

                    curY += 3;
                }
            }


            //하단 이미지
            if (bottomImage != null)
            {
                g.DrawImage(bottomImage, this.Width / 2F - bottomImage.Width / 2F, curY);
                curY += (bottomImage.Height);
            }
        }

        List<RectangleF> link_rect_list = null;
        private void drawPhraseBlock(PhraseType currentPhraseType, Graphics g, ref float curX, float curY, string subString)
        {
            Brush brush = null;
            Font font = null;

            if (!string.IsNullOrWhiteSpace(subString))
            {
                if (currentPhraseType == PhraseType.PLAIN)
                {
                    brush = new SolidBrush(Color.FromArgb(100, 100, 100));
                    font = new System.Drawing.Font("돋음", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                else if (currentPhraseType == PhraseType.EMPHASIS)
                {
                    brush = new SolidBrush(Color.FromArgb(0x00, 0x8f, 0xd5));
                    font = new Font("돋음", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                else if (currentPhraseType == PhraseType.WARNING)
                {
                    brush = new SolidBrush(Color.FromArgb(0xea, 0x23, 0x1a));
                    font = new Font("돋음", 11F, FontStyle.Regular, GraphicsUnit.Pixel);
                }
                else if (currentPhraseType == PhraseType.LINK)
                {
                    brush = new SolidBrush(Color.FromArgb(0x00, 0x8f, 0xd5));
                    font = new Font("돋음", 11F, FontStyle.Underline, GraphicsUnit.Pixel);
                }

                drawShadowString(g, subString, font, brush, curX, curY);

                SizeF partSize = g.MeasureString(subString, font);
                if (currentPhraseType == PhraseType.LINK)
                {
                    RectangleF rect = new RectangleF(curX, curY, partSize.Width, partSize.Height);
                    link_rect_list.Add(rect);
                }

                curX += partSize.Width;
            }
        }

        public static void alert(string msg, bool showClingLogo = false)
        {
            frmAlertPopup popup = new frmAlertPopup(new Size(350, 205), Resources.ico_error_b, msg, null, new OKButtonSet(), null);
            popup.TopMost = true;
            if (showClingLogo) popup.showClingLogo = true;

            Form parentForm = null;
            if (Application.OpenForms.Count > 0)
            {
                //가장 최근에 열린 폼의 child로 설정!
                parentForm = Application.OpenForms[Application.OpenForms.Count - 1];
                Console.WriteLine("ALERT PARENT FORM: " + parentForm);
            }
            else
            {
                if ((Program.mainframe != null) && (!Program.mainframe.IsDisposed))
                {
                    parentForm = Program.mainframe;
                }
            }


            if (parentForm != null)
            {
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.ShowDialog(parentForm);
            }
            else
            {
                popup.StartPosition = FormStartPosition.CenterScreen;
                popup.ShowDialog();
            }
        }
    }
}
