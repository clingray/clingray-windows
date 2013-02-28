using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.forms;

namespace ClingClient.common
{
    public class ControlLocator
    {

        public static void moveParentTopLeft(Control control, int xMargin, int yMargin)
        {
            control.Left = xMargin;
            control.Top = yMargin;
        }

        public static void moveParentBottomLeft(Control control, Control parent, int xMargin, int yMargin)
        {
            control.Left = xMargin;
            control.Top = parent.Height - control.Height - yMargin;

            if (parent is frmCommonPopup)
            {
                control.Top -= frmCommonPopup.shadowGap;
            }
        }

        public static void moveParentBottomRight(Control control, Control parent, int xMargin, int yMargin)
        {
            control.Left = parent.Width - control.Width - xMargin;
            control.Top = parent.Height - control.Height - yMargin;

            if (parent is frmCommonPopup)
            {
                control.Left -= frmCommonPopup.shadowGap;
                control.Top -= frmCommonPopup.shadowGap;
            }
        }

        public static void moveParentTopRight(Control control, Control parent, int xMargin, int yMargin)
        {
            control.Left = parent.Width - control.Width - xMargin;
            control.Top = yMargin;

            if (parent is frmCommonPopup)
            {
                control.Left -= frmCommonPopup.shadowGap;
            }
        }

        public static void moveParentRight(Control control, Control parent, int xMargin)
        {
            control.Left = parent.Width - control.Width - xMargin;

            if (parent is frmCommonPopup)
            {
                control.Left -= frmCommonPopup.shadowGap;
            }
        }

        public static void moveBelow(Control control, Control preControl, int yMargin)
        {
            control.Top = preControl.Top + preControl.Height + yMargin;
        }

        public static void moveRight(Control control, Control preControl, int xMargin)
        {
            control.Left = preControl.Left + preControl.Width + xMargin;
            control.Top = preControl.Top + Convert.ToInt32((preControl.Height / 2F) - (control.Height / 2F));
        }

        public static void moveLeft(Control control, Control preControl, int xMargin)
        {
            control.Left = preControl.Left - control.Width - xMargin;
            control.Top = preControl.Top + (preControl.Height / 2) - (control.Height / 2);
        }
    }
}
