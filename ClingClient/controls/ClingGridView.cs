using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using ClingClient.controls;

namespace ClingClient
{
    public partial class ClingGridView : ClingScrollContainer
    {
        public List<ClingControlBase> itemList = new List<ClingControlBase>();

        public int cellMarginHeight { get; set; }
        public int cellMarginWidth { get; set; }

        public ClingGridView() : base(0,20)
        {
            InitializeComponent();

            this.cellMarginHeight = 10;
            this.cellMarginWidth = 20;
        }

        public void showItemList()
        {
            this.clearControl();

            int x = 0;
            int y = 0;

            foreach (ClingControlBase item in itemList)
            {
                item.Location = new Point(x, y);
                this.addControl(item);

                x += (item.Width + cellMarginWidth);
                if ((x + item.Width) > this.Width)
                {
                    x = 0;
                    y += (item.Height + cellMarginHeight);
                }
            }
        }
    }
}
