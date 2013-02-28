using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using ClingClient.utilities;

namespace ClingClient {
    class NinepatchDrawer {
        public Bitmap topLeftBmp { get; set; }
        public Bitmap topCenterBmp { get; set; }
        public Bitmap topRightBmp { get; set; }
        public Bitmap midLeftBmp { get; set; }
        public Bitmap midCenterBmp { get; set; }
        public Bitmap midRightBmp { get; set; }
        public Bitmap botLeftBmp { get; set; }
        public Bitmap botCenterBmp { get; set; }
        public Bitmap botRightBmp { get; set; }

        public NinepatchDrawer() {
        }

        public void drawNinepatch(Size contextSize, Graphics g) {
            int w = contextSize.Width;
            int h = contextSize.Height;

            ImageAttributes attributes = new ImageAttributes();
            attributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Tile);

            Rectangle stretchable;

            if (topLeftBmp != null && topCenterBmp != null && topRightBmp != null
                && botLeftBmp != null && botCenterBmp != null && botRightBmp != null
                && midLeftBmp != null && midCenterBmp != null && midRightBmp != null)
            {
                // Top
                Point topStartingPoint = new Point(0, 0);
                stretchable = HorizontalStretchableImageDrawer.getCenterStretchable(contextSize, topStartingPoint, topLeftBmp, topRightBmp);
                g.DrawImage(topLeftBmp, topStartingPoint);
                g.DrawImage(topCenterBmp, stretchable,
                    0, 0, topCenterBmp.Width, topCenterBmp.Height, GraphicsUnit.Pixel, attributes);
                g.DrawImage(topRightBmp, stretchable.Right, topStartingPoint.Y);

                // Bottom
                Point bottomStartingPoint = new Point(0, h - botLeftBmp.Height);
                stretchable = HorizontalStretchableImageDrawer.getCenterStretchable(contextSize, bottomStartingPoint, botLeftBmp, botRightBmp);
                g.DrawImage(botLeftBmp, bottomStartingPoint);
                g.DrawImage(botCenterBmp, stretchable,
                    0, 0, botCenterBmp.Width, botCenterBmp.Height, GraphicsUnit.Pixel, attributes);
                g.DrawImage(botRightBmp, stretchable.Right, bottomStartingPoint.Y);

                // Middle left
                stretchable.X = 0;
                stretchable.Y = topLeftBmp.Height;
                stretchable.Width = midLeftBmp.Width;
                stretchable.Height = h - (topLeftBmp.Height + botLeftBmp.Height);
                g.DrawImage(midLeftBmp, stretchable,
                    0, 0, midLeftBmp.Width, midLeftBmp.Height, GraphicsUnit.Pixel, attributes);

                // Middle Center
                stretchable.X = topLeftBmp.Width;
                stretchable.Y = topCenterBmp.Height;
                stretchable.Width = w - (midLeftBmp.Width + midRightBmp.Width);
                stretchable.Height = h - (topCenterBmp.Height + botCenterBmp.Height);
                g.DrawImage(midCenterBmp, stretchable,
                    0, 0, midCenterBmp.Width, midCenterBmp.Height, GraphicsUnit.Pixel, attributes);

                // Middle Right
                stretchable.X = w - midRightBmp.Width;
                stretchable.Y = topRightBmp.Height;
                stretchable.Width = midRightBmp.Width;
                stretchable.Height = h - (topRightBmp.Height + botRightBmp.Height);
                g.DrawImage(midRightBmp, stretchable,
                    0, 0, midRightBmp.Width, midRightBmp.Height, GraphicsUnit.Pixel, attributes);
            }
        }
    }
}
