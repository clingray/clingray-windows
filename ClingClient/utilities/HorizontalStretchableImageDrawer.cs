using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace ClingClient.utilities {
    class HorizontalStretchableImageDrawer {
        public Bitmap leftBmp { get; set; }
        public Bitmap centerBmp { get; set; }
        public Bitmap rightBmp { get; set; }

        public HorizontalStretchableImageDrawer() {
        }

        public void drawStretchableImage(Size contextSize, Graphics g) {
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Tile);

            Rectangle stretchable;

            Point startingPoint = new Point(0, 0);
            stretchable = getCenterStretchable(contextSize, startingPoint, leftBmp, rightBmp);
            g.DrawImage(leftBmp, startingPoint);
            g.DrawImage(centerBmp, stretchable,
                0, 0, centerBmp.Width, centerBmp.Height, GraphicsUnit.Pixel, attributes);
            g.DrawImage(rightBmp, stretchable.Right, startingPoint.Y);
        }

        public static Rectangle getCenterStretchable(Size totalSize, Point startingPoint, Bitmap leftBmp, Bitmap rightBmp) {
            Point stretchStartPoint = new Point(startingPoint.X + leftBmp.Width, startingPoint.Y);
            Rectangle stretchable = new Rectangle(stretchStartPoint,
                                        new Size(totalSize.Width - startingPoint.X,
                                                leftBmp.Height));
            stretchable.Width -= leftBmp.Width + rightBmp.Width;
            return stretchable;
        }
    }
}
