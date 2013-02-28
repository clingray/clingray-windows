using System.Drawing;
using System.Windows.Forms;

namespace ClingClient.controls
{
    public partial class ClingImage : ClingControlBase
    {
        Bitmap _bgImage;
        public Bitmap bgImage
        {
            get
            {
                return _bgImage;
            }
            set
            {
                _bgImage = value;
                this.Invalidate();
            }
        }

        public ClingImage(Bitmap bgImage)
        {
            InitializeComponent();
            this.bgImage = bgImage;

            this.Size = bgImage.Size;
            this.Paint += new PaintEventHandler(ClingImage_Paint);
        }

        void ClingImage_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            drawImage(g, bgImage, 0, 0);
        }
    }
}
