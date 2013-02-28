using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClingClient.utilities
{
    class ToolTipHelper
    {
        static List<ToolTip> tooltip_list = new List<ToolTip>();

        public static void add(Control c, string msg)
        {
            removeToolTip(c);

            ToolTip tip = new ToolTip();
            tip.SetToolTip(c, msg);

            tooltip_list.Add(tip);

            //Console.WriteLine("add.toolTip count:" + tooltip_list.Count());
        }

        static void removeToolTip(Control c)
        {
            foreach(ToolTip t in tooltip_list.FindAll(o => (!string.IsNullOrWhiteSpace(o.GetToolTip(c)))))
            {
                t.RemoveAll();
                tooltip_list.Remove(t);

                //Console.WriteLine("removeFromControl.toolTip count:" + tooltip_list.Count());
            }
        }

        public static void removeFromControl(Control parentControl)
        {
            removeToolTip(parentControl);

            foreach (Control c in parentControl.Controls)
            {
                removeFromControl(c);
            }
        }
    }
}
