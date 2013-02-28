using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClingClientEngine;
using ClingClient.subForms;
using ClingClient.Properties;
using ClingClient.utilities;

namespace ClingClient.forms
{
    public partial class subProjectList : ClingControlBase, subFormInterface
    {
        //다름 subform을 갔다가 다시 back했을때 상태를 유지하기 위해 필요!
        static bool isShowAllProjectChecked = false;        //모든 프로젝트 보기 모드인지 여부
        static bool isOrderByTimeChecked = true;           //프로젝트 리스트가 시간순 정렬모드 인지 여부

        ClingGridView lvProject;
        ClingCheckBox chkShowAll1;
        ClingCheckBox chkShowAll2;
        ClingCheckBox chkShowManaged1;
        ClingCheckBox chkShowManaged2;
        ClingCheckBox chkOrderByTime;

        public static Bitmap[] CDJacketForProjectList = new Bitmap[] { 
            Resources.disk_01_normal,
            Resources.disk_02_normal,
            Resources.disk_03_normal,
            Resources.disk_04_normal,
            Resources.disk_05_normal,
            Resources.disk_06_normal,
            Resources.disk_07_normal,
            Resources.disk_08_normal,
            Resources.disk_09_normal,
            Resources.disk_10_normal
        };
        
        public subProjectList()
        {
            InitializeComponent();

            setControls();
            this.AllowDrop = true;
            this.Resize += new EventHandler(subProjectList_Resize);
            //this.DragEnter += new DragEventHandler(subProjectList_DragEnter);
            //this.DragDrop += new DragEventHandler(subProjectList_DragDrop);

            refreshProjectList();
        }

        /*
        void subProjectList_DragEnter(object sender, DragEventArgs e)
        {
            bool shouldAllow = true;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                foreach (string file in files)
                {
                    if (!Directory.Exists(file))
                    {
                        shouldAllow = false;
                        break;
                    }
                }
            }
            else
            {
                shouldAllow = false;
            }

            e.Effect = shouldAllow ? DragDropEffects.All : DragDropEffects.None;
        }

        void subProjectList_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            
        }
        */

        void subProjectList_Resize(object sender, EventArgs e)
        {
            //control들 위치 재배치!
            lvProject.Width = this.Width - lvProject.Left;
            lvProject.Height = this.Height - lvProject.Top;
            lvProject.showItemList();

            chkShowAll1.moveParentTopLeft(10, 0);
            chkShowAll2.moveRight(chkShowAll1, 0);
            chkShowManaged1.moveRight(chkShowAll2, 10);
            chkShowManaged2.moveRight(chkShowManaged1, 0);

            chkOrderByTime.moveParentTopRight(this, 20+31+3, 9);

            //배경 다시 그리기
            makeBgImage();
            this.Invalidate();
        }

        

        void makeBgImage()
        {
            bool applyTiling = true;

            Graphics g = getDC();

            Bitmap bmpMiddleLeft = Properties.Resources.bg_mid_l;
            Bitmap bmpMiddleRight = Properties.Resources.bg_mid_r;
            Bitmap bmpMiddleMiddle = Properties.Resources.bg_mid_m;

            Bitmap bmpOrderByTime = Properties.Resources.txt_sort_time_on;
            Bitmap bmpOrderByName = Properties.Resources.txt_sort_name_on;

            g.DrawImage(bmpMiddleLeft, 0, 0);
            g.DrawImage(bmpMiddleRight, this.Width - bmpMiddleRight.Width, 0);
            if (applyTiling)
            {
                for (int x = bmpMiddleLeft.Width; x < (this.Width - bmpMiddleRight.Width); x += bmpMiddleMiddle.Width)
                {
                    g.DrawImage(bmpMiddleMiddle, x, 0);
                }
            }
            //단색처리
            //g.FillRectangle(new SolidBrush(Color.FromArgb(24, 25, 27)), bmpMiddleLeft.Width, bmpTopLeft.Height, this.Width - (bmpMiddleRight.Width + bmpMiddleLeft.Width), bmpMiddleMiddle.Height);


            //이름순
            g.DrawImage(bmpOrderByName, this.Width - bmpOrderByName.Width - 20, 13);

            //시간순
            g.DrawImage(bmpOrderByTime, this.Width - bmpOrderByName.Width - 20 - bmpOrderByTime.Width - 6 - 43, 13);
        }

        

        public void refreshProjectList()
        {
            Cursor.Current = Cursors.WaitCursor;
            
            //툴팁 memory leak 방지를 위한 수동 dispose 
            ToolTipHelper.removeFromControl(lvProject);     



            //리스트뷰에 add
            lvProject.itemList.Clear();

            //신규 프로젝트추가
            {
                ClingProjectInfo item = new ClingProjectInfo(null);
                item.bmpCDJacket = null;
                
                item.onProjectInfoClick += new EventHandler(item_onProjectInfoClick);
                lvProject.itemList.Add(item);
            }

            //시간순/이름순 정렬
            IOrderedEnumerable<ClingRepo> sortedList = chkOrderByTime.checkValue ?
                ClingRayEngine.instance.repositories.repositories.OrderByDescending(o => o.lastModifiedTime) :
                ClingRayEngine.instance.repositories.repositories.OrderBy(o => o.name);

            foreach (ClingRepo repo in sortedList)
            {
                if(chkShowManaged1.checkValue)  //관리중인 프로젝트만 보기
                {
                    if (!repo.hasWorkTree()) continue;  //연결안된놈은 pass~
                }

                RepositoryContext.Status repoStatus = new RepoHelper().getTotalRepoStatus(repo);

                //잘못된 repo meta 삭제.
                if (repoStatus==RepositoryContext.Status.UNKNOWN)
                {
                    ClingRayEngine.instance.removeRepository(repo.uuid);
                    continue;
                }
                
                ClingProjectInfo item = new ClingProjectInfo(repo);
                item.onProjectListShouldReLoad += new EventHandler(item_onProjectListShouldReLoad);
                RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(repo);
                

                //정상적인 repo인지 체크
                if (context.isValidRepository())
                {
                    item.onProjectInfoClick += new EventHandler(item_onProjectInfoClick);
                    item.bmpCDJacket = getCoverImage(repo);

                    if (repoStatus.HasFlag(RepositoryContext.Status.CHANGED)) item.setModifiedStatus();
                }
                else
                {
                    item.bmpCDJacket = Resources.btn_disk_error;
                }


                lvProject.itemList.Add(item);
            }

            lvProject.showItemList();
            GC.Collect();

            Cursor.Current = Cursors.Default;
        }

        

       

        void item_onProjectListShouldReLoad(object sender, EventArgs e)
        {
            //작업기록 중단일때는 프로젝트 모두 보기 모드로 자동 전환!
            if ((sender is string) && sender.ToString().Equals("STOP"))
            {
                if (!chkShowAll1.checkValue)
                {
                    chkShowAll1.checkValue = true;
                    return;     //체크값 바뀌면 자동으로 reload 되므로 아래 코드 실행 안되게 return 시켜버림!
                }
            }

            refreshProjectList();
        }

        void setControls()
        {
            lvProject = new ClingGridView();
            lvProject.Top = 50;
            lvProject.Left = 15;
            //lvProject.BackColor = Color.Red;



            //프로젝트 보기 모드 설정
            //모두보기
            //불
            chkShowAll1 = new ClingCheckBox();
            chkShowAll1.setImages(Properties.Resources.ico_view_on, Properties.Resources.ico_view_off);
            chkShowAll1.checkValue = isShowAllProjectChecked;      //CheckChanged 이벤트 설정전에 셋팅 해야함!!! (안그러면 loadProject가 계속 불림!)
            chkShowAll1.CheckChanged += new EventHandler(chkShowAll1_CheckChanged);

            //text
            chkShowAll2 = new ClingCheckBox();
            chkShowAll2.setImages(Properties.Resources.txt_view_all_normal, Properties.Resources.txt_view_all_dimmed);
            chkShowAll2.CheckChanged += new EventHandler(chkShowAll2_CheckChanged);


            //기록중인 프로젝트
            //불
            chkShowManaged1 = new ClingCheckBox();
            chkShowManaged1.setImages(Properties.Resources.ico_view_on, Properties.Resources.ico_view_off);
            chkShowManaged1.checkValue = !isShowAllProjectChecked;      //CheckChanged 이벤트 설정전에 셋팅 해야함!!! (안그러면 loadProject가 계속 불림!)
            chkShowManaged1.CheckChanged += new EventHandler(chkShowManaged1_CheckChanged);

            //text
            chkShowManaged2 = new ClingCheckBox();
            chkShowManaged2.setImages(Properties.Resources.txt_view_writing_normal, Properties.Resources.txt_view_writing_dimmed);
            chkShowManaged2.CheckChanged += new EventHandler(chkShowManaged2_CheckChanged);


            //시간순/이름순 체크박스
            chkOrderByTime = new ClingCheckBox();
            chkOrderByTime.setImages(Properties.Resources.btn_sort_left, Properties.Resources.btn_sort_right);
            chkOrderByTime.checkValue = isOrderByTimeChecked;   //CheckChanged 이벤트 설정전에 셋팅 해야함!!! (안그러면 loadProject가 계속 불림!)
            chkOrderByTime.CheckChanged += new EventHandler(chkOrderByTime_CheckChanged);


            this.Controls.Add(chkShowAll1);
            this.Controls.Add(chkShowAll2);
            this.Controls.Add(chkShowManaged1);
            this.Controls.Add(chkShowManaged2);
            this.Controls.Add(lvProject);
            this.Controls.Add(chkOrderByTime);
        }

        void chkOrderByTime_CheckChanged(object sender, EventArgs e)
        {
            isOrderByTimeChecked = chkOrderByTime.checkValue;
            refreshProjectList();
        }

        void changeShowOption(object sender, bool fromShowAll)
        {
            ClingCheckBox c = (ClingCheckBox)sender;

            if (fromShowAll)
            {
                isShowAllProjectChecked = c.checkValue;

                chkShowAll1.checkValue = c.checkValue;
                chkShowAll2.checkValue = c.checkValue;

                chkShowManaged1.checkValue = !c.checkValue;
                chkShowManaged2.checkValue = !c.checkValue;
            }
            else
            {
                isShowAllProjectChecked = !c.checkValue;

                chkShowAll1.checkValue = !c.checkValue;
                chkShowAll2.checkValue = !c.checkValue;

                chkShowManaged1.checkValue = c.checkValue;
                chkShowManaged2.checkValue = c.checkValue;
            }
        }

        void chkShowManaged1_CheckChanged(object sender, EventArgs e)
        {
            changeShowOption(sender, false);
        }

        void chkShowManaged2_CheckChanged(object sender, EventArgs e)
        {
            changeShowOption(sender, false);
        }

        void chkShowAll1_CheckChanged(object sender, EventArgs e)
        {
            changeShowOption(sender, true);
            refreshProjectList();
        }

        void chkShowAll2_CheckChanged(object sender, EventArgs e)
        {
            changeShowOption(sender, true);
        }

        void item_onProjectInfoClick(object sender, EventArgs e)
        {
            ClingProjectInfo item = sender as ClingProjectInfo;
            if (item.repo == null)
            {
                //프로젝트 추가

                frmAddProjectPopup popup = new frmAddProjectPopup();
                if (popup.ShowDialog(this) == DialogResult.OK)
                {
                    if (frmAddProjectPopup.exitString == null)  //프로젝트 추가 성공
                    {
                        refreshProjectList();
                    }
                    else //이미 있는 프로젝트 선택해서, 커밋 리스트 보기로 전환
                    {
                        RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(frmAddProjectPopup.exitString);
                        ((frmFrame)this.Parent.Parent).showCommitList(context.repoInfo);
                    }

                }
            }
            else
            {
                //버젼 리스트 보기로 이동
                ((frmFrame)this.Parent.Parent).showCommitList(item.repo);
            }
        }

        Bitmap getCoverImage(ClingRepo repo)
        {
            //인덱스 체크
            int idx = ((repo.coverImageIndex+1)>CDJacketForProjectList.Length) ? 0 : repo.coverImageIndex;
            return CDJacketForProjectList[idx];
        }

        public void goBack()
        {
        }

        public void goRefresh()
        {
            refreshProjectList();
        }
    }
}
