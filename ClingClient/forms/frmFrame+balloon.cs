using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.forms;

namespace ClingClient
{
    public partial class frmFrame : Form
    {
        private Queue<BalloonTipData> balloonQueue = new Queue<BalloonTipData>();
        private BalloonTipData currentBalloonTipData;
        private bool isDisplayingBalloonTip = false;

        void registerBalloonTipHandler()
        {
            trayIcon.BalloonTipShown += new EventHandler(trayIcon_BalloonTipShown);
            trayIcon.BalloonTipClicked += new EventHandler(trayIcon_BalloonTipClicked);
            trayIcon.BalloonTipClosed += new EventHandler(trayIcon_BalloonTipClosed);
        }

        void enqueueBalloonTipData(BalloonTipData balloonTipData)
        {
            balloonQueue.Enqueue(balloonTipData);

            if (!isDisplayingBalloonTip)
            {
                showNextBalloonTip();
            }
        }

        void showNextBalloonTip()
        {
            if (balloonQueue.Count > 0)
            {                
                currentBalloonTipData = balloonQueue.Dequeue();

                trayIcon.BalloonTipIcon = currentBalloonTipData.icon;
                trayIcon.BalloonTipTitle = currentBalloonTipData.title;
                trayIcon.BalloonTipText = currentBalloonTipData.text;

                trayIcon.ShowBalloonTip(currentBalloonTipData.timeout);
            }
        }

        void showBalloonTipImmediately(BalloonTipData balloonTipData)
        {
            discardAll();
            enqueueBalloonTipData(balloonTipData);

            showNextBalloonTip();
        }

        void discardAll()
        {
            balloonQueue.Clear();
        }

        void trayIcon_BalloonTipShown(object sender, EventArgs e)
        {
            isDisplayingBalloonTip = true;
        }

        void trayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (currentBalloonTipData != null && currentBalloonTipData.clickHandler != null)
            {
                currentBalloonTipData.clickHandler(sender, e);
            }
            trayIcon_BalloonTipClosed(sender, e);
        }

        void trayIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (currentBalloonTipData != null)
            {
                trayIcon.BalloonTipClicked -= currentBalloonTipData.clickHandler;
            }

            if (balloonQueue.Count == 0)
            {
                isDisplayingBalloonTip = false;
                //currentBalloonTipData = null;
            }
            else
            {
                showNextBalloonTip();
            }
        }
    }

    class BalloonTipData
    {
        public const int DEFAULT_TIMEOUT = 1500;
        public ToolTipIcon icon { get; set; }
        public string title { get; set; }
        public string text { get; set; }
        public int timeout { get; set; }
        public object tag { get; set; }

        public EventHandler clickHandler { get; set; }

        public BalloonTipData(ToolTipIcon icon, string title, string text, EventHandler clickHandler = null, int timeout = DEFAULT_TIMEOUT)
        {
            this.icon = icon;
            this.title = title;
            this.text = text;
            this.clickHandler = clickHandler;
            this.timeout = timeout;
        }
    }
}
