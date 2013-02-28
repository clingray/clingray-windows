using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClingClient
{
    public partial class ClingCheckBox : ClingControlBase
    {
        bool _checkValue;
        public bool checkValue
        {
            get
            {
                return _checkValue;
            }

            set
            {
                if (_checkValue != value)
                {
                    _checkValue = value;
                    changeImage();

                    if( (CheckChanged != null) && canRaiseCheckChangedEvent )
                    {
                        CheckChanged(this, null);
                    }
                }
            }
        }

        Bitmap _bmpOff = null;
        public Bitmap bmpOff
        {
            get
            {
                return _bmpOff;
            }
            set
            {
                _bmpOff = value;
                this.Size = value.Size;
                changeImage();
            }
        }

        Bitmap _bmpOn = null;
        public Bitmap bmpOn
        {
            get
            {
                return _bmpOn;
            }
            set
            {
                _bmpOn = value;
                this.Size = value.Size;
                changeImage();
            }
        }

        public bool canRaiseCheckChangedEvent { get; set; }
        public event EventHandler CheckChanged;

        public ClingCheckBox()
        {
            InitializeComponent();

            this.canRaiseCheckChangedEvent = true;
            this.TabStop = false;
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ClingButton_MouseUp);
        }

        public void setImages(Bitmap on, Bitmap off)
        {
            bmpOn = on;
            bmpOff = off;
        }

        void changeImage()
        {
            if (checkValue)
            {
                if (bmpOn != null) this.BackgroundImage = bmpOn;
            }
            else
            {
                if (bmpOff != null) this.BackgroundImage = bmpOff;
            }
        }

        private void ClingButton_MouseUp(object sender, MouseEventArgs e)
        {
            checkValue = !checkValue;
        }
    }
}
