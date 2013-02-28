using System.Collections.Generic;
using System.Drawing;
using ClingClient.commitList;
using System.Windows.Forms;
using System;

namespace ClingClient.controls {
    public partial class ClingTableView : ClingScrollContainer
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        } 

        public const int DEFAULT_MARGIN_HEIGHT = 6;
        public int cellMarginHeight { get; set; }

        public List<ClingControlBase> itemList = new List<ClingControlBase>();

        public ClingTableView() : base(10,0) 
        {
            InitializeComponent();

            this.cellMarginHeight = DEFAULT_MARGIN_HEIGHT;
        }

        // http://nickstips.wordpress.com/2010/03/03/c-panel-resets-scroll-position-after-focus-is-lost-and-regained/
        //protected override Point ScrollToControl(Control activeControl)
        //{ 
        //    return this.DisplayRectangle.Location;
        //}

        private const int INITIAL_Y = 0;
        public void refreshItems() {

            this.clearControl();

            int x = 3;
            int y = INITIAL_Y;

            foreach (ClingControlBase item in itemList) {
                item.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                // 아아아아아아아악 어쩔수 없이 예외 처리.
                if (!(item is ClingButton))
                {
                    item.Width = this.getContentsWidth - this.Margin.Left - this.Margin.Right;
                }

                item.Location = new Point(x, y);

                this.addControl(item);

                // 아아아아아아아악 어쩔수 없이 예외 처리.
                if (item is ClingButton)
                {
                    x = (this.getContentsWidth - item.Width) / 2;
                    item.Location = new Point(x, y);
                }

                y += item.Height + cellMarginHeight;
            }
        }

        public void addItems() {

        }
    }
}
