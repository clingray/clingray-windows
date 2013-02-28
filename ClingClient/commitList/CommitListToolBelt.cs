using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.utilities;
using ClingClient.Properties;

namespace ClingClient.commitList {
    public partial class CommitListToolBelt : UserControl {
        public enum ViewType {
            LIST,
            TREE
        }

        public string repositoryStatus {
            get {
                return status.Text;
            }
            set {
                status.Text = value;
            }
        }

        HorizontalStretchableImageDrawer bgDrawer = new HorizontalStretchableImageDrawer();

        ViewType _viewType = ViewType.LIST;
        public ViewType viewType {
            get {
                return _viewType;
            }
            set {
                if (value == ViewType.LIST) {
                    viewTypeToggle.checkValue = false;
                    viewByListImage.Image = Resources.txt_sort_list_on;
                    viewByTreeImage.Image = Resources.txt_sort_tree_disable;
                } else {
                    viewTypeToggle.checkValue = true;
                    viewByListImage.Image = Resources.txt_sort_list_disable;
                    viewByTreeImage.Image = Resources.txt_sort_tree_on;
                }
            }
        }

        public CommitListToolBelt() {
            InitializeComponent();
            bgDrawer.leftBmp = Resources.bg_mid_l;
            bgDrawer.centerBmp = Resources.bg_mid_m;
            bgDrawer.rightBmp = Resources.bg_mid_r;

            //tree나올때까지 disable처리!
            viewTypeToggle.Enabled = false;
        }

        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            bgDrawer.drawStretchableImage(this.Size, e.Graphics);
        }

        private void viewTypeToggle_CheckChanged(object sender, EventArgs e) {
            ClingCheckBox toggle = (ClingCheckBox)sender;
            viewType = toggle.checkValue ? ViewType.TREE : ViewType.LIST;
        }
    }
}
