using System;
using System.Collections.Generic;
using ClingClient.forms;
using System.Drawing;
using System.Windows.Forms;
using ClingClientEngine;
using System.IO;
using ClingClient.utilities;
using ClingClient.common;
using ClingClient.Properties;

namespace ClingClient.popup {
    public class SharedPopupController {
        public enum Result {
            Success,
            Error
        }
        public delegate void UIUpdateDelegate(Result result);

        public static DialogResult showUnlinkWorkTreeDialog(string workTreePath, IWin32Window owner = null, UIUpdateDelegate uiUpdateDelegate = null) {
            frmAlertPopup questionPopup = new frmAlertPopup(
                new Size(375, 253),
                Properties.Resources.ico_error_b,
                "작업 기록을 중단하시겠습니까?",
                "이 프로젝트의 작업 기록을 더 이상 할 수 없게 됩니다.\n백업은 남아있으며 $그 어떤 파일도 삭제되지 않습니다.",
                new OKCancelButtonSet(), null
            );

            DialogResult answer;

            if (owner != null) {
                answer = questionPopup.ShowDialog(owner);
            } else {
                answer = questionPopup.ShowDialog();
            }

            if (answer == DialogResult.OK) {
                Cursor.Current = Cursors.WaitCursor;

                try {
                    ClingRayEngine.instance.unlinkWorkTree(workTreePath);
                    WorkTreeMonitoring.instance.stopMonitoringByWorkTree(workTreePath);

                    if (uiUpdateDelegate != null) {
                        uiUpdateDelegate(Result.Success);
                    }

                    frmAlertPopup alertPopup = new frmAlertPopup(
                        new Size(398, 430),
                        Properties.Resources.ico_error_b,
                        "작업 기록이 중단되었습니다.",
                        "백업은 남아있으며 그 어떤 파일도 삭제되지 않았습니다.\n기록을 다시 시작하시려면 목록에서 $'작업 재시작' 버튼$을 선택해 주세요.",
                        new CloseButtonSet(), Properties.Resources.img_info_rewrite
                    );

                    if (owner != null) {
                        alertPopup.ShowDialog(owner);
                    } else {
                        alertPopup.ShowDialog();
                    }                    
                } catch (Exception ee) {
                    if (uiUpdateDelegate != null) {
                        uiUpdateDelegate(Result.Error);
                    }

                    frmAlertPopup.alert(ee.Message);
                } finally {
                    Cursor.Current = Cursors.Default;
                }
            } else 
            {
                
            }

            return answer;
        }
        

        static bool commitResult = true;
        static int stopCommit = 0;      //0:통과 1:대기 2:중지
        static void commitFiles(object param, frmProgress popProgress)
        {
            stopCommit = 0;
            commitResult = true;
            Cursor.Current = Cursors.WaitCursor;

            object[] paramList = param as object[];

            RepositoryContext currentRepoContext = paramList[0] as RepositoryContext;
            string commitMsg = paramList[1] as string;


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
            if (commitResult && (stopCommit==0))
            {
                //자동으로 첫 커밋 시키기!
                commitResult = currentRepoContext.commit(commitMsg);
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
                //커밋에 실패했으므로 
                //git add 한거 제거 해야함!

                //TODO
                //근데 꼭 add한거 없애야 하나?? 걍 둬도 담 커밋할때 알아서 업댓 될껀데??
            }

            Cursor.Current = Cursors.Default;
        }

        public static DialogResult showCommitDialog(RepositoryContext context, IWin32Window owner = null, UIUpdateDelegate uiUpdateDelegate = null) {
            DialogResult answer = DialogResult.Cancel;

            List<Form> modalForms = ApplicationUtils.getModalForms();
            if (modalForms.Count > 0)
            {
                foreach (Form modalForm in modalForms)
                {
                    if (modalForm is frmCommitMessagePopup)
                    {
                        modalForm.Dispose();
                    }
                    else if (modalForm is frmProgress)
                    {
                        modalForm.BringToFront();
                        modalForm.Focus();

                        return DialogResult.Cancel;
                    }
                }
            }

            if (context == null)
            {
                frmAlertPopup.alert("프로젝트 상태가 올바르지 않습니다.");
                return DialogResult.Cancel;
            }

            if (!context.isChanged()) {
                frmAlertPopup alert = new frmAlertPopup(
                    new Size(400, 250),

                    Properties.Resources.ico_warning,
                    "프로젝트 변경사항이 없습니다.",
                    "작업 전이거나 작업 후 저장을 하지 않은 곡입니다.\n기록을 남기기 위해서는 DAW에서 반드시 저장이 필요합니다.",
                    new CloseButtonSet(), null
                );
                alert.showClingLogo = true;

                if (owner != null)
                {
                    alert.ShowDialog(owner);
                }
                else
                {
                    alert.ShowDialog();
                }
                
            } else {
                frmCommitMessagePopup popup = new frmCommitMessagePopup();
                popup.repositoryName = StringUtils.getStringWithEllipsis(context.repoInfo.name, 17);
                if (owner != null)
                {
                    answer = popup.ShowDialog(owner);
                }
                else
                {
                    answer = popup.ShowDialog();
                }

                if (answer == DialogResult.OK) 
                {
                    //프로그레스 처리!
                    object[] paramList = new object[] { context, popup.txtCommitMsg.getText() };
                    ProgressHelper progress = new ProgressHelper(owner, "파일 정보를 기록중입니다..\n잠시만 기다려주세요...");
                    progress.CancelClick += new EventHandler(popProgress_CancelClick);
                    progress.shot(new ProgressHelper.LONG_WORK_FUNC(commitFiles), paramList);  //modal action 임!

                    if (commitResult)
                    {
                        //frmAlertPopup.alert("프로젝트 작업이 기록되었습니다.");

                        if (uiUpdateDelegate != null)
                        {
                            uiUpdateDelegate(Result.Success);
                        }
                    }
                    else
                    {
                        frmAlertPopup.alert("작업기록 남기기에 실패하였습니다!");
                    }


                } else 
                {
                    
                }
            }

            return answer;
        }

        static void popProgress_CancelClick(object sender, EventArgs e)
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

        //동기화 할려는데, 로컬commit 되지 않은 변경사항이 있는 경우!
        static DialogResult checkLocalChangeBeforeSync(RepositoryContext context, IWin32Window dialogOwner)
        {
            try
            {
                if (context.isChanged())
                {
                    frmAlertPopup alert = new frmAlertPopup(
                        new Size(425, 259),
                        Resources.ico_error_b,
                        "아직 기록되지 않은 작업이 있습니다.",
                        "현재 작업 상황을 기록하지 않고 진행하면 최근 프로젝트 변경사항은 보관되지 않고 사라지게 됩니다.\n온라인 백업을 진행하기 전에 지금의 작업상황을 기록해두시겠습니까?"
                        , new CheckoutButtonSet(),
                        null
                    );
                    DialogResult result = alert.ShowDialog(dialogOwner);

                    if (result == DialogResult.Yes)  //commit
                    {
                        return showCommitDialog(context, dialogOwner, null);
                    }
                    else if (result == DialogResult.Ignore)  //변경사항 무시하고 이전버젼으로 checkout
                    {
                        if (context.rollbackToHEAD())
                        {
                            return DialogResult.OK;
                        }
                        else
                        {
                            frmAlertPopup.alert("프로젝트의 변경사항을 되돌리는데 실패하였습니다.");
                            return DialogResult.Cancel;
                        }
                    }
                    else //cancel
                    {
                        return DialogResult.Cancel;
                    }
                }

                return DialogResult.OK;
            }
            catch (Exception ee)
            {
                frmAlertPopup.alert(ee.Message);
            }

            return DialogResult.Cancel;
        }

        

       

        public static string showSelectNewWorkTree(IWin32Window owner)
        {
            bool dummyBool;
            return showSelectNewWorkTree(owner, false, out dummyBool);
        }

        public static string showSelectNewWorkTree(IWin32Window owner, out bool shouldDeleteWorkTree)
        {
            return showSelectNewWorkTree(owner, true, out shouldDeleteWorkTree);
        }

        private static string showSelectNewWorkTree(IWin32Window owner, bool showThreeway, out bool shouldDeleteWorkTree)
        {
            string workTreePath = null;
            shouldDeleteWorkTree = false;

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.Description = "작업 기록을 다시 시작할 프로젝트(곡) 폴더를 지정해주세요.";
            if (dlg.ShowDialog() == DialogResult.OK) workTreePath = dlg.SelectedPath;
            else return null;

            if (!ClingRayEngine.instance.canSetWorkingFolder(workTreePath))
            {
                frmAlertPopup.alert("이 폴더는 작업 폴더로 사용할 수 없습니다.\n다른 폴더를 선택해 주세요.");
                return null;
            }

            ClingRepo repository = ClingRayEngine.instance.repositories.findRepositoryByWorkTree(workTreePath);
            if (repository != null)
            {
                frmAlertPopup.alert("현재 작업 관리 중인 폴더입니다.\n다른 폴더를 선택해주세요.");
                return null;
            }

            if (clsUtil.isDirectoryInUse(workTreePath))
            {
                frmAlertPopup.alert("작업 폴더가 다른 프로그램에 의해 사용 중입니다.\n다른 프로그램을 종료 후 다시 시도해 주세요.");
                return null;
            }

            if (!SystemUtils.isDirecotryEmpty(workTreePath))
            {
                string message;
                if (showThreeway)
                {
                    message = "$무시하고 진행$을 선택할 경우 ^해당 폴더의 내용을 지우고^ 최신 버전의 내용으로 덮어 씁니다.\n $확인$을 선택할 경우 $해당 폴더의 내용을 그대로 사용$합니다.";
                }
                else
                {
                    message = "계속 진행할 경우 해당 폴더의 내용을 지우고 최신 버전의 내용으로 덮어 씁니다.";
                }

                DialogResult result = new frmAlertPopup(new Size(398, 253),
                    Properties.Resources.ico_error_b,
                    "해당 폴더에 파일이 존재합니다.",
                    message,
                    showThreeway ? (IAlertButtonSet)new LinkFolderButtonSet() : (IAlertButtonSet)new OKCancelButtonSet(), 
                    null).ShowDialog(owner);

                if (showThreeway)
                {
                    if (result == DialogResult.Yes)
                    {
                        // do nothing.
                    }
                    else if (result == DialogResult.Ignore)
                    {
                        shouldDeleteWorkTree = true;
                    }
                    else
                    {
                        workTreePath = null;
                    }
                }
                else
                {
                    if (result == DialogResult.OK)
                    {
                        shouldDeleteWorkTree = true;
                    }
                    else
                    {
                        workTreePath = null;
                    }
                }
            }

            return workTreePath;
        }
    }
}
