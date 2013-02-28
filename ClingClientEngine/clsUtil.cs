using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace ClingClientEngine
{
    public class clsUtil
    {
        public static void writeLog(string msg)
        {
            FileStream fs = new FileStream(@"\cling.log", FileMode.Append);
            msg = "[" + DateTime.Now + "] " + msg;
            byte[] buf = Encoding.UTF8.GetBytes(msg + "\r\n");
            fs.Write(buf, 0, buf.Length);
            fs.Close();
        }

        public static string[] getAllFileList(String rootDir)
        {
            string[] paths = Directory.GetFiles(rootDir);

            string[] dirs = Directory.GetDirectories(rootDir);
            foreach (String dir in dirs)
            {
                //숨김폴더 제외!
                if (((new DirectoryInfo(dir)).Attributes & FileAttributes.Hidden) == 0)
                {
                    //Console.WriteLine(dir);
                    paths = paths.Concat(getAllFileList(dir)).ToArray();
                }
            }

            return paths;
        }

        public static bool deleteRecursively(string dirPath) {
            bool succeeded = true;
            try
            {
                deleteRecursively(new DirectoryInfo(dirPath));
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
                succeeded = false;
            }

            return succeeded;
        }

        public static void deleteRecursively(DirectoryInfo dirInfo)
        {
            foreach (var subDir in dirInfo.GetDirectories())
            {
                deleteRecursively(subDir);
            }

            foreach (var file in dirInfo.GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }

            dirInfo.Delete();
        }

        public static int runCommandLine(string applicationPath, string applicationArguments) {
            string lastErrorMsg;
            return runCommandLine(applicationPath, applicationArguments, out lastErrorMsg);
        }

        public static int runCommandLine(string applicationPath, string applicationArguments, out string lastErrorMsg)
        {
            Console.WriteLine("\n-----------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine(applicationPath + " " + applicationArguments + "\n");

            // noisyblue : 아래 부분은 child process 에서 stdout, stderr 에서 출력되는 데이터가 많을 경우 
            // 프로세스가 hang 되는 문제가 있어 false 처리합니다.
            bool showCmdOut = false;
            Process ProcessObj = new Process();

            ProcessObj.StartInfo.FileName = applicationPath;
            ProcessObj.StartInfo.Arguments = applicationArguments;

            ProcessObj.StartInfo.UseShellExecute = false;
            ProcessObj.StartInfo.CreateNoWindow = true;
            
            ProcessObj.StartInfo.RedirectStandardOutput = showCmdOut;
            ProcessObj.StartInfo.RedirectStandardError = showCmdOut;

            ProcessObj.Start();
            //ProcessObj.PriorityClass = ProcessPriorityClass.High;

            ProcessObj.WaitForExit();

            
            //Console.WriteLine("ProcessObj.ExitCode:" + ProcessObj.ExitCode);

            
            // noisyblue 아래 부분은 child process 에서 stdout, stderr 에서 출력되는 데이터가 많을 경우 프로세스가 hang 되는 문제가 있어
            // 주석 처리합니다.
            if (showCmdOut)
            {
                string Result = ProcessObj.StandardOutput.ReadToEnd();
                Result += ProcessObj.StandardError.ReadToEnd();
                Console.WriteLine("result:" + Result);
            }
            

            lastErrorMsg = string.Empty;

            Console.WriteLine("===============================================================================================================================================");

            return ProcessObj.ExitCode;
        }

        public static int runCommandLineWithWindow(string applicationPath, string applicationArguments)
        {
            Process ProcessObj = new Process();

            ProcessObj.StartInfo.FileName = applicationPath;
            ProcessObj.StartInfo.Arguments = applicationArguments;

            ProcessObj.Start();
            ProcessObj.WaitForExit();

            Console.WriteLine("runCommandLineWithWindow:" + applicationPath + " " + applicationArguments);
            Console.WriteLine("ProcessObj.ExitCode:" + ProcessObj.ExitCode);

            return ProcessObj.ExitCode;
        }

        public static string getOSCompatiblePathName(string name) {
            // TODO : 구현 필요.
            return name;
        }

        public static bool isDirectoryInUse(string directoryPath)
        {
            return isDirectoryInUse(new DirectoryInfo(directoryPath));
        }

        private static bool isDirectoryInUse(DirectoryInfo dirInfo)
        {
            bool isInUse = false;

            //if (!Directory.Exists(dirInfo.FullName)) return false;

            foreach (var subDir in dirInfo.GetDirectories())
            {
                isInUse = isDirectoryInUse(subDir);

                if (isInUse) break;
            }

            if (!isInUse)
            {
                foreach (FileInfo file in dirInfo.GetFiles())
                {
                    try
                    {
                        using (Stream stream = new FileStream(file.FullName, FileMode.Open))
                        {
                            // do nothing.
                        }
                    }
                    catch
                    {
                        isInUse = true;
                    }

                    if (isInUse) break;
                }
            }

            return isInUse;
        }

        public static bool directoryContainsAnotherDirectory(string path1, string path2)
        {
            path1 = Path.GetFullPath(path1);
            path2 = Path.GetFullPath(path2);

            return path1.Contains(path2);
        }
    }
}
