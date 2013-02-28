using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClingClient.controls;

namespace ClingClient.commitList {
    public partial class MonthSeparatorBar : ClingControlBase
    {
        public MonthSeparatorBar(DateTime date) {
            InitializeComponent();

            if (date != null) {
                monthLabel.Text = date.ToString("yyyy.MM");
            }
        }
    }
}
