using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClingClient.forms;
using System.Windows.Forms;
using System.Drawing;

namespace ClingClient.popup
{
    public interface IAlertButtonSet
    {
        void addButtons(frmCommonPopup popup);
        void arrangeButtons(frmCommonPopup popup);
    }

    public class CheckoutButtonSet : ThreewayButtonSet
    {
        public CheckoutButtonSet() {
            doAlternativeNormal = Properties.Resources.btn_popoup_write_normal;
            doAlternativePressed = Properties.Resources.btn_popoup_write_press;
            
            ignoreNormal = Properties.Resources.btn_popup_ignore_normal;
            ignorePressed = Properties.Resources.btn_popup_ignore_press;

            cancelNormal = Properties.Resources.btn_popup_cancel_normal;
            cancelPressed = Properties.Resources.btn_popup_cancel_press;
        }
    }

    public class LinkFolderButtonSet : ThreewayButtonSet
    {
        public LinkFolderButtonSet()
        {
            doAlternativeNormal = Properties.Resources.btn_popup_ok_norml;
            doAlternativePressed = Properties.Resources.btn_popup_ok_press;
            
            ignoreNormal = Properties.Resources.btn_popup_ignore_normal;
            ignorePressed = Properties.Resources.btn_popup_ignore_press;

            cancelNormal = Properties.Resources.btn_popup_cancel_normal;
            cancelPressed = Properties.Resources.btn_popup_cancel_press;
        }
    }

    /// <summary>
    /// 대체 동작, 무시하고 진행, 취소 3가지의 동작에 해당하는 button set.
    /// </summary>
    public class ThreewayButtonSet : IAlertButtonSet
    {
        protected ClingButton btnDoAlternative, btnIgnore, btnCancel;

        public Bitmap doAlternativeNormal { get; set; }
        public Bitmap doAlternativePressed { get; set; }
        public Bitmap ignoreNormal { get; set; }
        public Bitmap ignorePressed { get; set; }
        public Bitmap cancelNormal { get; set; }
        public Bitmap cancelPressed { get; set; }

        public void addButtons(frmCommonPopup popup)
        {
            btnDoAlternative = new ClingButton();
            btnDoAlternative.setImages(doAlternativeNormal, doAlternativePressed);
            btnDoAlternative.ButtonClick += new EventHandler(onButtonClick);

            btnIgnore = new ClingButton();
            btnIgnore.setImages(ignoreNormal, ignorePressed);
            btnIgnore.ButtonClick += new EventHandler(onButtonClick);

            btnCancel = new ClingButton();
            btnCancel.setImages(cancelNormal, cancelPressed);
            btnCancel.ButtonClick += new EventHandler(onButtonClick);

            popup.Controls.Add(btnDoAlternative);
            popup.Controls.Add(btnIgnore);
            popup.Controls.Add(btnCancel);
        }

        public void arrangeButtons(frmCommonPopup popup)
        {
            btnIgnore.moveParentBottomLeft(popup, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);
            btnCancel.moveParentBottomRight(popup, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);
            btnDoAlternative.moveLeft(btnCancel, 5);
        }

        void onButtonClick(object sender, EventArgs e)
        {
            frmCommonPopup popup = (frmCommonPopup)((Control)sender).Parent;
            DialogResult dialogResult = DialogResult.No;
            if (sender == btnDoAlternative)
            {
                dialogResult = DialogResult.Yes;
            }
            else if (sender == btnIgnore)
            {
                dialogResult = DialogResult.Ignore;
            }
            else if (sender == btnCancel)
            {
                dialogResult = DialogResult.Cancel;
            }

            popup.DialogResult = dialogResult;
        }
    }

    public class OKButtonSet : IAlertButtonSet
    {
        ClingButton btnOK;

        public void addButtons(frmCommonPopup popup)
        {
            btnOK = new ClingButton();
            btnOK.setImages(Properties.Resources.btn_popup_ok_norml,
                Properties.Resources.btn_popup_ok_press,
                Properties.Resources.btn_popup_ok_norml);
            btnOK.ButtonClick += new EventHandler(onButtonClick);

            popup.Controls.Add(btnOK);

            popup.AcceptButton = btnOK;
        }

        public void arrangeButtons(frmCommonPopup popup)
        {
            btnOK.moveParentBottomRight(popup, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);
        }

        void onButtonClick(object sender, EventArgs e)
        {
            frmCommonPopup popup = (frmCommonPopup)((Control)sender).Parent;
            DialogResult dialogResult = DialogResult.No;
            if (sender == btnOK)
            {
                dialogResult = DialogResult.OK;
            }

            popup.DialogResult = dialogResult;
        }
    }

    public class CloseButtonSet : IAlertButtonSet
    {
        ClingButton btnClose;

        public void addButtons(frmCommonPopup popup)
        {
            //닫기
            btnClose = new ClingButton();
            btnClose.setImages(Properties.Resources.btn_popup_close_normal,
                Properties.Resources.btn_popup_close_press,
                Properties.Resources.btn_popup_close_normal);
            btnClose.ButtonClick += new EventHandler(btnClose_ButtonClick);

            popup.Controls.Add(btnClose);
        }

        public void arrangeButtons(frmCommonPopup popup)
        {
            //닫기
            btnClose.moveParentBottomRight(popup, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);
        }

        void btnClose_ButtonClick(object sender, EventArgs e)
        {
            frmCommonPopup popup = (frmCommonPopup)((Control)sender).Parent;
            popup.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }

    public class OKCancelButtonSet : IAlertButtonSet
    {
        ClingButton btnCancel, btnOK;

        public void addButtons(frmCommonPopup popup)
        {
            //취소버튼
            btnCancel = new ClingButton();
            btnCancel.setImages(Properties.Resources.btn_popup_cancel_normal,
                Properties.Resources.btn_popup_cancel_press,
                Properties.Resources.btn_popup_cancel_normal);
            btnCancel.ButtonClick += new EventHandler(btnCancel_ButtonClick);

            //등록
            btnOK = new ClingButton();
            btnOK.setImages(Properties.Resources.btn_popup_ok_norml,
                Properties.Resources.btn_popup_ok_press,
                Properties.Resources.btn_popup_ok_norml);
            btnOK.ButtonClick += new EventHandler(btnOK_ButtonClick);

            popup.Controls.Add(btnOK);
            popup.Controls.Add(btnCancel);
        }

        public void arrangeButtons(frmCommonPopup popup)
        {
            //취소버튼
            btnCancel.moveParentBottomRight(popup, Program.POPUP_BOTTOM_BUTTON_H_MARGIN, Program.POPUP_BOTTOM_BUTTON_V_MARGIN);

            //등록
            btnOK.moveLeft(btnCancel, 5);
        }

        void btnCancel_ButtonClick(object sender, EventArgs e)
        {
            frmCommonPopup popup = (frmCommonPopup)((Control)sender).Parent;
            popup.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        void btnOK_ButtonClick(object sender, EventArgs e)
        {
            frmCommonPopup popup = (frmCommonPopup)((Control)sender).Parent;
            popup.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        
    }
}
