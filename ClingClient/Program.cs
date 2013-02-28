using System;
using System.Windows.Forms;
using ClingClient.forms;
using System.Threading;
using ClingUpdater;
using ClingClientEngine;
using ClingClient.popup;
using System.IO;
using ClingClient.ipc;
using System.Xml.Serialization;
using ClingClient.updater;
using ClingClient.utilities;
using ClingClient.Properties;

namespace ClingClient
{
    public static class Program
    {
        /**
         * link folder as working directory.
         * arg[1]
         **/
        private const string ARG_LINK_FOLDER = "link";
        private const string ARG_UNLINK_FOLDER = "unlink";
        private const string ARG_CREATE_VERSION = "commit";
        private const string ARG_RESTORE_VERSION = "checkout";
        private const string ARG_HELP = "help";
        private const string ARG_ABOUT = "about";

        public static string commiter_email = "ohw@clingray.com";

        private static Form currentForm = null;

        public static frmFrame mainframe = null;

        public static int POPUP_BOTTOM_BUTTON_V_MARGIN = 14;      //원래는 15인데, 팝업의 그림자 때문에 14로 해야 가운데로 보인다!
        public static int POPUP_BOTTOM_BUTTON_H_MARGIN = 31;

        /**
         * args[0] : command type
         * args[1] : command parameter
         * args[2] : from where?
         **/
        [STAThread]
        static void Main(string[] args)
        {
            updateSettings();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            

             // 처음으로 실행되는 인스턴스인 경우 named pipe server 를 실행한다.
            if (!ApplicationUtils.isAnotherInstanceRunning())
            {
                startPipeServer();
            }

            ClingInitialAction initialAction = getActionFromArguments(args);
            // Mainframe 을 보여주지 않는 조건
            // 1. 다른 instance 가 실행 중인 경우.
            // 2. Command-line argument 가 주어진 경우.
            bool shouldShowMainframe = !ApplicationUtils.isAnotherInstanceRunning() && initialAction == ClingInitialAction.NO_ACTION;
            
            if (initialAction == ClingInitialAction.NO_ACTION)
            {
                if (ApplicationUtils.isAnotherInstanceRunning())
                {
                    ClingStartupCommand startupCmd = getStartupCommand(args, ClingInitialAction.BRING_TO_FRONT);
                    sendCommandToPipeServer(startupCmd);
                }
            }
            else
            {
                ClingStartupCommand startupCmd = getStartupCommand(args, initialAction);
                if (ApplicationUtils.isAnotherInstanceRunning())
                {
                    sendCommandToPipeServer(startupCmd);
                }
                else
                {
                    executeCommand(startupCmd);
                }
            }

            if (shouldShowMainframe)
            {
                // About & Update-checking
                showModalAboutPopup();


                // Update
                if (updateInfo != null && updateInfo.exists)
                {
                    DialogResult result = showModalUpdatePopup();

                    // Critical update 에 대한 무시(DialogResult.Abort) 의 경우 애플리케이션 종료.
                    if (currentForm.DialogResult == DialogResult.Abort)
                    {
                        finalizeApplication();
                        return;
                    }
                }

                currentForm = new frmFrame();
                Application.Run(currentForm);
            }

            finalizeApplication();
        }

        private static void updateSettings()
        {
            if (Settings.Default.settingsUpdateRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.settingsUpdateRequired = false;
                Settings.Default.Save();
            }
        }

        private static void finalizeApplication()
        {
            if (pipeServer != null)
            {
                pipeServer.Stop();
            }
        }

        #region Popups
        private static DialogResult showModalPopup(Form form)
        {
            currentForm = form;
            return currentForm.ShowDialog();
        }

        private static DialogResult showModalUpdatePopup()
        {
            frmUpdatePopup updatePopup = new frmUpdatePopup(updateInfo);
            updatePopup.TopMost = true;
            updatePopup.ShowInTaskbar = true;

            return showModalPopup(updatePopup);
        }

        private static DialogResult showModalAboutPopup()
        {
            frmAboutPopup aboutForm = new frmAboutPopup(frmAboutPopup.DisplayMode.SPLASH);
            aboutForm.StartPosition = FormStartPosition.CenterScreen;

            // Check update during splash.
            new Thread(new ThreadStart(checkUpdate)).Start();

            return showModalPopup(aboutForm);
        }
        #endregion Popups


        #region IPC
        private const string PIPE_NAME = "ClingPipe";

        static NamedPipeServer pipeServer;
        private static void startPipeServer()
        {
            pipeServer = new NamedPipeServer(PIPE_NAME);
            pipeServer.OnReceivedMessage += new EventHandler<ReceivedMessageEventArgs>(onReceivedMessage);
            pipeServer.Start();
        }

        static void onReceivedMessage(object sender, ReceivedMessageEventArgs e)
        {
            ClingStartupCommand command = null;

            try
            {
                using (StringReader reader = new StringReader(e.Message))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ClingStartupCommand));
                    command = (ClingStartupCommand)deserializer.Deserialize(reader);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error during parsing xml");
            }

            if (command != null)
            {
                executeCommand(command);
            }
        }

        private static void sendCommandToPipeServer(ClingStartupCommand command)
        {
            NamedPipeClient pipeClient = new NamedPipeClient(PIPE_NAME);
            pipeClient.Start();

            XmlSerializer serializer = new XmlSerializer(typeof(ClingStartupCommand));
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, command);

                pipeClient.Write(writer.ToString());
                //pipeClient.Stop();
            }
        }

        private static void executeCommand(ClingStartupCommand command)
        {
            ClingInitialAction initialAction = (ClingInitialAction)command.action;
            if (initialAction == ClingInitialAction.ABOUT)
            {
                frmAboutPopup about = new frmAboutPopup(frmAboutPopup.DisplayMode.ABOUT);
                about.TopMost = true;
                about.StartPosition = FormStartPosition.CenterScreen;
                Application.Run(about);
            }
            else if (initialAction == ClingInitialAction.LINK_AS_WORKTREE)
            {
                frmAddProjectPopup popup = new frmAddProjectPopup();
                popup.TopMost = true;
                popup.setWorkTreePath(command.parameter);
                popup.ShowDialog();
            }
            else if (initialAction == ClingInitialAction.UNLINK_WORKTREE)
            {
                SharedPopupController.showUnlinkWorkTreeDialog(command.parameter);
            }
            else if (initialAction == ClingInitialAction.CREATE_VERSION)
            {
                RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(command.parameter);
                SharedPopupController.showCommitDialog(context);
            }
            else if (initialAction == ClingInitialAction.RESTORE_VERSION)
            {
                ClingRepo repository = ClingRayEngine.instance.repositories.findRepositoryByWorkTree(command.parameter);
                if (repository != null)
                {
                    if (mainframe == null)
                    {
                        mainframe = new frmFrame();
                        mainframe.startupRepo = repository;
                        Application.Run(mainframe);
                    }
                    else
                    {
                        mainframe.showCommitList(repository);
                    }
                }
            }
            else if (initialAction == ClingInitialAction.HELP)
            {
                frmAlertPopup.alert("[ALPHA] Comming soon!");
            }
            else if (initialAction == ClingInitialAction.BRING_TO_FRONT)
            {
                if (mainframe != null)
                {
                    mainframe.restoreWindow();
                }
                else if (currentForm != null)
                {
                    currentForm.Focus();
                    currentForm.BringToFront();
                }
            }

            if (mainframe == null)
            {
                if (initialAction != ClingInitialAction.NO_ACTION &&
                    initialAction != ClingInitialAction.BRING_TO_FRONT &&
                    initialAction != ClingInitialAction.RESTORE_VERSION)
                {
                    mainframe = new frmFrame();
                    mainframe.isStartHidden = true;
                    Application.Run(mainframe);
                }
            }
        }
        #endregion IPC

        #region Update checking
        private static UpdateInfo updateInfo;
        private static void checkUpdate()
        {
            UpdateChecker checker = ClingUpdateHelper.getUpdateChecker();
            checker.checkUpdate(onUpdateCheckCompleted);
        }

        public static void onUpdateCheckCompleted(UpdateInfo updateInfo)
        {
            Program.updateInfo = updateInfo;
            if (currentForm is frmAboutPopup)
            {
                currentForm.Invoke(new MethodInvoker(currentForm.Close));
            }
        }
        #endregion Update checking

        #region Startup command
        public enum ClingInitialAction : int
        {
            NO_ACTION,
            LINK_AS_WORKTREE,
            UNLINK_WORKTREE,
            CREATE_VERSION,
            RESTORE_VERSION,
            HELP,
            ABOUT,
            BRING_TO_FRONT
        };

        private static ClingStartupCommand getStartupCommand(string[] args, ClingInitialAction initialAction)
        {
            ClingStartupCommand command = new ClingStartupCommand();
            command.action = (int)initialAction;

            if (initialAction == ClingInitialAction.LINK_AS_WORKTREE)
            {
                command.parameter = args[1];
            }
            else if (initialAction == ClingInitialAction.UNLINK_WORKTREE)
            {
                command.parameter = args[1];
            }
            else if (initialAction == ClingInitialAction.CREATE_VERSION)
            {
                command.parameter = args[1];
            }
            else if (initialAction == ClingInitialAction.RESTORE_VERSION)
            {
                command.parameter = args[1];
            }

            return command;
        }

        private static ClingInitialAction getActionFromArguments(string[] args)
        {
            ClingInitialAction initialAction = ClingInitialAction.NO_ACTION;

            if (args.Length > 0)
            {
                if (args[0].Equals(ARG_LINK_FOLDER))
                {
                    initialAction = ClingInitialAction.LINK_AS_WORKTREE;
                }
                else if (args[0].Equals(ARG_UNLINK_FOLDER))
                {
                    initialAction = ClingInitialAction.UNLINK_WORKTREE;
                }
                else if (args[0].Equals(ARG_CREATE_VERSION))
                {
                    initialAction = ClingInitialAction.CREATE_VERSION;
                }
                else if (args[0].Equals(ARG_RESTORE_VERSION))
                {
                    initialAction = ClingInitialAction.RESTORE_VERSION;
                }
                else if (args[0].Equals(ARG_HELP))
                {
                    initialAction = ClingInitialAction.HELP;
                }
                else if (args[0].Equals(ARG_ABOUT))
                {
                    initialAction = ClingInitialAction.ABOUT;
                }
            }

            return initialAction;
        }
        #endregion Startup command
    }
}
