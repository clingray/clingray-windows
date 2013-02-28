using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;

namespace ClingClient.utilities
{
    class ApplicationUtils
    {
        #region Modal window
        public static bool isDisplayingModalWindow()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Modal)
                {
                    return true;
                }
            }

            return false;
        }

        public static Form getFirstModalForm()
        {
            foreach (Form f in Application.OpenForms)
            {
                if (f.Modal)
                {
                    return f;
                }
            }

            return null;
        }

        public static List<Form> getModalForms()
        {
            List<Form> modalForms = new List<Form>();

            foreach (Form f in Application.OpenForms)
            {
                if (f.Modal)
                {
                    modalForms.Add(f);
                }
            }

            return modalForms;
        }

        #endregion Modal window

        #region Application instance checking
        static bool triedMutexCreation = false;
        static bool anotherInstanceRunning;
        static Mutex instanceMutex;
        public static bool isAnotherInstanceRunning()
        {
            if (!triedMutexCreation)
            {
                bool createdNew;
                instanceMutex = new Mutex(true, "ClingrayInstanceLock", out createdNew);
                anotherInstanceRunning = !createdNew;

                triedMutexCreation = true;
            }

            return anotherInstanceRunning; 
        }
        #endregion Application instance checking

        public static string getApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}
