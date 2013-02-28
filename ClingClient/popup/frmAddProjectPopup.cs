using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ClingClientEngine;
using ClingClient.popup;
using ClingClient.common;
using ClingClient.utilities;

namespace ClingClient.forms {
    public partial class frmAddProjectPopup : frmCommonPopup {
        ClingTextBox txtPath;
        ClingTextBox txtProjectName;
        ClingTextBox txtCommitMsg;
        ClingButton btnSearchPath;
        ClingButton btnCancel;
        ClingButton btnRegister;

        static string prePath;

        public frmAddProjectPopup() {
            InitializeComponent();
            setControls();

            showClingLogo = true;
            showCloseButton = true;
            showButtonLine = true;
            
            this.Resize += new EventHandler(frmAddProjectPopup_Resize);
            this.Load += new EventHandler(frmAddProjectPopup_Load);
        }

        void frmAddProjectPopup_Load(object sender, EventArgs e) {
            this.Size = new System.Drawing.Size(462, 272);
        }

        void setControls() {
            //프로젝트 폴더 텍스트 박스
            txtPath = new ClingTextBox("프로젝트 폴더 경로");
            //txtPath.Font = new System.Drawing.Font("돋음", 14F, GraphicsUnit.Pixel);
            txtPath.Size = new System.Drawing.Size(340, 26);
            txtPath.TabIndex = 0;


            //프로젝트 명
            txtProjectName = new ClingTextBox("곡 명");
            txtProjectName.Size = new System.Drawing.Size(400, 26);
            txtProjectName.TabIndex = 1;


            //첫 커밋 메시지
            txtCommitMsg = new ClingTextBox("지금 이 곡의 작업단계는? 예) 녹음 후 편집 완료, 러프 믹스");
            //txtCommitMsg.Font = new System.Drawing.Font("돋음", 14F, GraphicsUnit.Pixel);
            txtCommitMsg.Size = new System.Drawing.Size(400, 26);
            txtCommitMsg.TabIndex = 2;


            //찾아보기 버튼
            btnSearchPath = new ClingButton();
            btnSearchPath.setImages(Properties.Resources.btn_search_normal,
                Properties.Resources.btn_search_press,
                Properties.Resources.btn_search_hover);
            btnSearchPath.ButtonClick += new EventHandler(btnSearchPath_ButtonClick);


            //취소버튼
            btnCancel = new ClingButton();
            btnCancel.setImages(Properties.Resources.btn_popup_cancel_normal,
                Properties.Resources.btn_popup_cancel_press,
                Properties.Resources.btn_popup_cancel_normal);
            btnCancel.ButtonClick += new EventHandler(btnCancel_ButtonClick);

            //등록
            btnRegister = new ClingButton();
            btnRegister.setImages(Properties.Resources.btn_popup_enter_normal,
                Properties.Resources.btn_popup_enter_press,
                Properties.Resources.btn_popup_enter_normal);
            btnRegister.ButtonClick += new EventHandler(btnRegister_ButtonClick);


            this.Controls.Add(txtPath);
            this.Controls.Add(txtProjectName);
            this.Controls.Add(txtCommitMsg);
            this.Controls.Add(btnSearchPath);
            this.Controls.Add(btnCancel);
            this.Controls.Add(btnRegister);

            this.AcceptButton = btnRegister;
        }


        public static string exitString = null;

        

        bool commitResult = true;
        int stopCommit = 0;      //0:통과 1:대기 2:중지
        void commitFiles(object param, frmProgress popProgress)
        {
            Cursor.Current = Cursors.WaitCursor;
            commitResult = true;
            stopCommit = 0;
            RepositoryContext currentRepoContext = param as RepositoryContext;

            //git add *

            /*
             * 수동으로 모든 파일을 긁어서 git add 하는 방식에서
             * repo에 untracked 파일만 조사해서 add하는 방식으로 개선!
             */

            //string[] fileList = clsUtil.getAllFileList(currentRepoContext.workTreePath);
            string[] fileList = currentRepoContext.loadUntrackedFiles();

            for (int i = 0; i < fileList.Length; i++)
            {
                //취소 대기
                while (stopCommit > 0)
                {
                    if (stopCommit == 2) break;
                    Application.DoEvents();
                }
                if (stopCommit == 2) break;


                double prgValue = ((i + 1) / (double)fileList.Length) * 100;

                popProgress.sendMsg(Path.GetFileName(fileList[i]), prgValue);
                if (!currentRepoContext.add(fileList[i]))
                {
                    commitResult = false;
                    break;
                }
            }


            //git commit
            if (commitResult && (stopCommit == 0))
            {
                //자동으로 첫 커밋 시키기!
                commitResult = currentRepoContext.commit(txtCommitMsg.getText(), false);
            }
            else
            {
                commitResult = false;
            }


            if (commitResult)
            {
                
            }
            else
            {
                //커밋에 실패했으므로 repo 만들어둔거 삭제
                ClingRayEngine.instance.removeRepository(currentRepoContext.repoInfo.uuid);
            }


            Cursor.Current = Cursors.Default;
        }

        void btnRegister_ButtonClick(object sender, EventArgs e) {
            if (txtPath.getText().Length == 0) {
                frmAlertPopup.alert("프로젝트 폴더 경로를 선택해 주세요.");
                return;
            }

            if (!Directory.Exists(txtPath.getText())) {
                frmAlertPopup.alert(txtPath.getText() + "\n존재하지 않는 경로 입니다!");
                return;
            }

            if (ClingRayEngine.instance.getWorkTreeStatus(txtPath.getText()) != RepositoryContext.Status.UNKNOWN)
            {
                this.Hide();

                frmAlertPopup popup = new frmAlertPopup(
                    new Size(398, 220),
                    Properties.Resources.ico_error_b,
                    "현재 작업기록 중인 프로젝트입니다.\n이 프로젝트의 기록으로 이동하시겠습니까?",
                    null,
                    new OKCancelButtonSet(), null
                );
                popup.showClingLogo = true;

                if (popup.ShowDialog(this) == DialogResult.OK)
                {
                    exitString = txtPath.getText();
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                }

                this.Show();
                return;
            }


            if (txtProjectName.getText().Length == 0) {
                frmAlertPopup.alert("프로젝트명을 입력하세요.");
                return;
            }

            if (txtCommitMsg.getText().Length == 0) {
                //첫 커밋 메시지는 없으면 자동으로 아래로 지정! (효정 요청사항)
                txtCommitMsg.setText("첫 번째 작업기록");
            }

            

            String workingDirPath = txtPath.getText();
            String workingDirName = txtProjectName.getText();


            try {
                Cursor.Current = Cursors.WaitCursor;

                RepositoryContext currentRepoContext = ClingRayEngine.instance.addRepository(workingDirName, workingDirPath, RepoHelper.makeCoverImageIndex());
                if (currentRepoContext != null) {

                    if (clsUtil.getAllFileList(workingDirPath).Length == 0)
                    {
                        //빈폴더인 경우
                        //파일이 있어야 커밋 되므로,, dummy 파일 하나 만든다!
                        File.Create(Path.Combine(workingDirPath, "CLING")).Dispose();

                        //dummy CLING 파일을 hidden 처리
                        FileInfo FI = new FileInfo(Path.Combine(workingDirPath, "CLING"));
                        FI.Attributes = FileAttributes.Hidden;
                    }


                    //commit 시작!

                    //프로그레스 처리!
                    ProgressHelper progress = new ProgressHelper(this, "파일 정보를 기록중입니다..\n잠시만 기다려주세요...");
                    progress.CancelClick += new EventHandler(popProgress_CancelClick);
                    progress.shot(new ProgressHelper.LONG_WORK_FUNC(commitFiles), currentRepoContext);  //modal action 임!


                    if (commitResult)
                    {
                        exitString = null;
                        WorkTreeMonitoring.instance.startMonitoringByWorkTree(txtPath.getText());
                        this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    }
                    else
                    {
                        frmAlertPopup.alert("작업기록을 남기는데 실패하였습니다!");
                    }

                    
                } else 
                {
                    throw new Exception("fail to add repository...");
                }
            } catch (Exception ee) {
                frmAlertPopup.alert(ee.Message);
            } finally {
                Cursor.Current = Cursors.Default;
            }
        }

        void popProgress_CancelClick(object sender, EventArgs e)
        {
            stopCommit = 1;

            frmAlertPopup popup = new frmAlertPopup(
                    new Size(300, 220),
                    Properties.Resources.ico_error_b,
                    "진행 중인 작업기록을 취소 하시겠습니까?",
                    null,
                    new OKCancelButtonSet(), null
                );
            popup.StartPosition = FormStartPosition.CenterParent;
            popup.TopMost = true;
            if (popup.ShowDialog((Form)sender) == DialogResult.OK)
            {
                stopCommit = 2;
            }
            else
            {
                stopCommit = 0;
            }
        }

        void btnCancel_ButtonClick(object sender, EventArgs e) {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        void btnSearchPath_ButtonClick(object sender, EventArgs e) {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.ShowDialog();

            String selectedDir = dlg.SelectedPath;
            if (!string.IsNullOrWhiteSpace(selectedDir))
            {
                if (!ClingRayEngine.instance.canSetWorkingFolder(selectedDir))
                {
                    frmAlertPopup.alert("이 폴더는 작업 폴더로 사용할 수 없습니다.\n다른 폴더를 선택해 주세요.");
                    return;
                }
                else
                {
                    setWorkTreePath(selectedDir);
                }
            }
        }

        public void setWorkTreePath(String workTreePath) {
            if ((workTreePath != null) && (workTreePath.Length > 0)) {
                txtPath.setText(workTreePath);
                txtProjectName.setText(Path.GetFileName(workTreePath));

                prePath = workTreePath;
            }
        }

        void frmAddProjectPopup_Resize(object sender, EventArgs e) {
            Point ptTitle = getContentTopLocation();

            //프로젝트 폴더 텍스트 박스
            txtPath.Location = new Point(ptTitle.X, (int)(ptTitle.Y + 32));


            //프로젝트 명
            txtProjectName.moveBelow(txtPath, 9);
            txtProjectName.Left = txtPath.Left;


            //첫 커밋 메시지
            txtCommitMsg.moveBelow(txtProjectName, 9);
            txtCommitMsg.Left = txtPath.Left;

            //찾아보기 버튼
            btnSearchPath.moveRight(txtPath, 3);
            btnSearchPath.Top -= 2;

            //취소버튼
            btnCancel.moveParentBottomRight(this, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);

            //등록
            btnRegister.moveLeft(btnCancel, 5);
        }

        protected override void makeBackgroundImage(Graphics g) {
            //타이틀
            {
                Font fontCmt1 = new System.Drawing.Font("돋음", 11F, FontStyle.Bold, GraphicsUnit.Pixel);
                g.DrawString("프로젝트 추가", fontCmt1, new SolidBrush(Color.Black), getContentTopLocation());
            }


            //프로젝트 경로는 곡 단위로 등록해 주세요
            {
                Point ptBottomCmt = getButtonCommentLocation();

                Bitmap bmpEx = Properties.Resources.ico_popup_info;

                //젤 왼쪽 느낌표 이미지
                g.DrawImage(bmpEx, ptBottomCmt);

                Font fontCmt1 = new System.Drawing.Font("돋음체", 8, GraphicsUnit.Point);
                float textLeft = ptBottomCmt.X + bmpEx.Width + 3;
                float textTop = ptBottomCmt.Y;
                string text1 = "프로젝트 경로는";
                string text2 = "곡 단위";
                string text3 = "로 등록해 주세요";

                g.DrawString(text1, fontCmt1, new SolidBrush(Color.FromArgb(0xa2, 0xa2, 0xa2)), textLeft, textTop);

                textLeft += g.MeasureString(text1, fontCmt1).Width;
                g.DrawString(text2, fontCmt1, new SolidBrush(Color.FromArgb(0x14, 0x9f, 0xe4)), textLeft, textTop);

                textLeft += g.MeasureString(text2, fontCmt1).Width;
                g.DrawString(text3, fontCmt1, new SolidBrush(Color.FromArgb(0xa2, 0xa2, 0xa2)), textLeft, textTop);

            }
        }
    }
}