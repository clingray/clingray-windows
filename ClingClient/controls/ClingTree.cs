using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ClingClient.controls
{
    public partial class ClingTree : UserControl
    {
        int TREE_SIZE = treeObj.MIN_TREE_SIZE;

        static int TREE_H_MARGIN = 50;
        static int TREE_V_MARGIN = 100;

        static Color selectedColor = Color.FromArgb(0x91, 0xba, 0x27);
        static Color normalColor = Color.FromArgb(0xef, 0xee, 0xee);
        static Pen normalArrowPen = null;
        static Pen normalLinePen = null;

        TreeContainer container = null;
        SortedList<int, treeObj> treeList = null;
        Bitmap paintBuffer = null;
        treeObj rootTree = null;
        treeObj selectedTree = null;

        static ClingTree()
        {
            normalLinePen = new Pen(Color.Gray, 5);

            normalArrowPen = new Pen(Color.Gray, 5);
            normalArrowPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
            normalArrowPen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
        }

        public ClingTree()
        {
            InitializeComponent();

            //더블 버퍼링
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.UpdateStyles();

            container = new TreeContainer();
            //container.BackColor = Color.White;
            container.Paint += new PaintEventHandler(ClingTree_Paint);
            container.MouseWheel += new MouseEventHandler(ClingTree_MouseWheel);
            container.MouseDown += new MouseEventHandler(container_MouseDown);
            container.MouseUp += new MouseEventHandler(container_MouseUp);
            container.MouseMove += new MouseEventHandler(container_MouseMove);

            this.Controls.Add(container);
        }

        void container_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDownPoint != Point.Empty)
            {
                Point newPoint = e.Location;

                int gapX = (newPoint.X - mouseDownPoint.X);
                int gapY = (newPoint.Y - mouseDownPoint.Y);

                container.Left += gapX;
                container.Top += gapY;
            }
        }

        void container_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.Default;
            mouseDownPoint = Point.Empty;
        }

        Point mouseDownPoint = Point.Empty;
        void container_MouseDown(object sender, MouseEventArgs e)
        {
            Cursor.Current = Cursors.SizeAll;
            mouseDownPoint = e.Location;
        }

        void ClingTree_MouseWheel(object sender, MouseEventArgs e)
        {
            //Console.WriteLine("delta:" + e.Delta);
            int gap = (e.Delta > 0) ? 10 : -10;

            if (((TREE_SIZE + gap) < treeObj.MIN_TREE_SIZE) || ((TREE_SIZE + gap) > treeObj.MAX_TREE_SIZE)) return;

            TREE_SIZE += gap;
            setTree(rootTree);
        }

        int getLeafCount(treeObj tree)
        {
            tree.leafCount = (tree.childList.Count == 0) ? 1 : 0;
            foreach (treeObj child in tree.childList)
            {
                tree.leafCount += getLeafCount(child);
            }

            //Console.WriteLine(tree.caption + ".leafCount:" + tree.leafCount);

            return tree.leafCount;
        }

        double DEGREE_TO_RADIAN(double degree)
        {
            return Math.PI * degree / 180.0;
        }

        void setTreePos(treeObj tree, int x, int y)
        {
            tree.size = new Size(tree.leafCount * (TREE_SIZE + TREE_H_MARGIN), TREE_SIZE);
            tree.leftTop = new Point(x, y);
            treeList.Add(tree.key, tree);

            //width로 gap을 유동적으로 조정
            //double avail_degree = 5;
            //int v_gap = (int)(tree.size.Width * Math.Tan(DEGREE_TO_RADIAN(avail_degree)));
            //if (v_gap < TREE_V_MARGIN) v_gap = TREE_V_MARGIN;
            int v_gap = TREE_V_MARGIN;

            for (int i = 0; i < tree.childList.Count; i++)
            {
                setTreePos(tree.childList[i], x, y + TREE_SIZE + v_gap);
                x += tree.childList[i].size.Width;
            }
        }

        void autoResize()
        {
            int max_x = treeList.Max(s => s.Value.right);
            int max_y = treeList.Max(s => s.Value.bottom);

            container.Width = max_x + TREE_H_MARGIN;
            container.Height = max_y + TREE_V_MARGIN;
        }

        public void setTree(treeObj rootTree)
        {
            //Console.WriteLine("setTree");

            this.rootTree = rootTree;
            treeList = new SortedList<int, treeObj>();

            getLeafCount(rootTree);
            setTreePos(rootTree, TREE_H_MARGIN, 20);
            autoResize();

            makePaintBitmap();
            container.Invalidate();
        }

        public treeObj getTree(int key)
        {
            if (treeList.ContainsKey(key)) return treeList[key];
            return null;
        }

        public treeObj selectTree(int key)
        {
            treeObj tree = getTree(key);
            if (tree != null)
            {
                tree.selected = true;
                if (selectedTree != null)
                {
                    selectedTree.selected = false;
                    selectedTree = null;
                }

                makePaintBitmap();
                container.Invalidate();
            }

            return tree;
        }

        void makePaintBitmap()
        {
            //Console.WriteLine("makePaintBitmap");

            if (paintBuffer != null)
            {
                paintBuffer.Dispose();
                paintBuffer = null;
                GC.Collect();
            }
            paintBuffer = new Bitmap(container.Width, container.Height);
            Graphics g = Graphics.FromImage(paintBuffer);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            //연결 라인
            foreach (treeObj tree in treeList.Values)
            {
                if (tree.childList.Count > 0)
                {
                    int x1 = tree.childList[0].center.X;
                    int x2 = tree.childList[tree.childList.Count - 1].center.X;
                    int x_mid = (x1 + x2) / 2;
                    int y = tree.childList[0].centerTop.Y - (TREE_V_MARGIN / 2);

                    g.DrawLine(normalLinePen, tree.centerBottom, new Point(x_mid, y));

                    g.DrawLine(normalLinePen,
                        new Point(x1, y),
                        new Point(x2, y)
                     );
                }

                if (tree.parent != null)
                {
                    g.DrawLine(normalArrowPen, new Point(tree.centerTop.X, tree.centerTop.Y - (TREE_V_MARGIN / 2)), tree.centerTop);
                }

            }

            //node
            Font captionFont = new Font("맑은 고딕", TREE_SIZE / 4, FontStyle.Regular, GraphicsUnit.Pixel);
            foreach (treeObj tree in treeList.Values)
            {
                //전체 사각형
                //g.FillRectangle(new SolidBrush(Color.Yellow), tree.rect);
                //g.DrawRectangle(Pens.Gray, tree.rect);


                //테두리 원
                Rectangle circleRect = new Rectangle(tree.rect.Left + tree.rect.Width / 2 - TREE_SIZE / 2, tree.rect.Top, TREE_SIZE, TREE_SIZE);
                g.FillEllipse(new SolidBrush(tree.selected ? selectedColor : normalColor), circleRect);
                g.DrawEllipse(Pens.Gray, circleRect);


                //가운데 글자
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                g.MeasureString(tree.caption, captionFont, circleRect.Size, sf);

                g.DrawString(tree.caption, captionFont, Brushes.Black, circleRect, sf);
            }
        }


        void ClingTree_Paint(object sender, PaintEventArgs e)
        {
            if (paintBuffer != null)
            {
                Graphics g = e.Graphics;
                g.DrawImage(paintBuffer, 0, 0, paintBuffer.Width, paintBuffer.Height);
            }
        }

        public class treeObj
        {
            public static int MIN_TREE_SIZE = 50;
            public static int MAX_TREE_SIZE = 500;

            static int keyIndex = 0;

            public int leafCount = 0;
            public int key;
            public int level;
            public bool selected { get; set; }
            public string caption { get; set; }
            public treeObj parent { get; set; }
            public List<treeObj> childList;

            public Point leftTop;
            public Size size;
            public int top
            {
                get
                {
                    return leftTop.Y;
                }

                set
                {
                    leftTop = new Point(leftTop.X, value);
                }
            }
            public int right
            {
                get
                {
                    return leftTop.X + size.Width;
                }
            }
            public int left
            {
                get
                {
                    return leftTop.X;
                }
            }
            public Point rightMiddle
            {
                get
                {
                    return new Point(right, center.Y);
                }
            }
            public Point leftMiddle
            {
                get
                {
                    return new Point(leftTop.X, center.Y);
                }
            }
            public Point rightBottom
            {
                get
                {
                    return new Point(right, bottom);
                }
            }
            public int bottom
            {
                get
                {
                    return leftTop.Y + size.Height;
                }
            }
            public Point centerBottom
            {
                get
                {
                    return new Point(center.X, bottom);
                }
            }
            public Rectangle rect
            {
                get
                {
                    return new Rectangle(leftTop, size);
                }

                set
                {
                    leftTop = new Point(value.Left, value.Top);
                    size = new Size(value.Width, value.Height);
                }
            }
            public Point center
            {
                get
                {
                    return new Point(leftTop.X + size.Width / 2, leftTop.Y + size.Height / 2);
                }

                set
                {
                    leftTop = new Point(value.X - size.Width / 2, value.Y - size.Height / 2);

                }
            }
            public Point centerTop
            {
                get
                {
                    return new Point(center.X, top);
                }

                set
                {
                    leftTop = new Point(value.X - size.Width / 2, value.Y);

                }
            }

            public treeObj(string caption)
            {
                this.size = new Size(MIN_TREE_SIZE, MIN_TREE_SIZE);
                this.key = keyIndex++;
                this.caption = caption;
                this.childList = new List<treeObj>();
            }

            public void addChild(treeObj child)
            {
                childList.Add(child);
                child.parent = this;
                child.level = this.level + 1;
            }

            bool Equals(treeObj other)
            {
                return (this.key == other.key);
            }
        }

        class TreeContainer : UserControl
        {
            public TreeContainer()
            {
                //더블 버퍼링
                this.SetStyle(ControlStyles.DoubleBuffer, true);
                this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                this.SetStyle(ControlStyles.UserPaint, true);
                this.UpdateStyles();
            }
        }
    }

}
