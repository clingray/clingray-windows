using System;
using ClingClient.popup;
using System.Windows.Forms;
using System.Threading;

namespace ClingClient.utilities
{
    class ProgressHelper
    {
        frmProgress popProgress;
        IWin32Window owner;

        public delegate void LONG_WORK_FUNC(object parm, frmProgress popProgress);
        LONG_WORK_FUNC func;

        public event EventHandler CancelClick;

        public ProgressHelper(IWin32Window owner, string title, bool showCloseButton = true, bool progressWithMsg = true)
        {
            this.owner = owner;

            popProgress = new frmProgress(title, progressWithMsg);
            popProgress.Show();
            popProgress.Hide();
            popProgress.Cursor = Cursors.WaitCursor;
            popProgress.CancelClick += new EventHandler(popProgress_CancelClick);
            popProgress.showCloseButton = showCloseButton;
        }

        void popProgress_CancelClick(object sender, EventArgs e)
        {
            if (CancelClick != null) CancelClick(sender, e);
        }

        public void shot(LONG_WORK_FUNC func, object param)
        {
            this.func = func;
            new Thread(new ParameterizedThreadStart(threadWork)).Start(param);
            popProgress.ShowDialog(owner);
        }

        void _closeProgress(object param)
        {
            //프로그레스 팝업 잔상이 남는 현상이 발생해서 아래와 같이 확실하게 죽여줬다!
            popProgress.Hide();
            popProgress.Close();
            popProgress = null;
            Application.DoEvents();
        }

        void threadWork(object parm)
        {
            func(parm, popProgress);
            popProgress.Invoke(new frmProgress.AFTERWORK(_closeProgress), new object());
        }
    }
}
