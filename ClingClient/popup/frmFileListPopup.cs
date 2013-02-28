using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClingClient.forms;
using ClingClientEngine;
using System.Collections;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace ClingClient.popup
{
    class frmFileListPopup : frmCommonPopup
    {
        RepositoryContext context;
        Commit commit;
        ArrayList fileList;
        ListView lvFile;

        public frmFileListPopup(RepositoryContext context, Commit commit)
        {
            this.context = context;
            this.commit = commit;

            this.showClingLogo = true;
            this.showCloseButton = true;
            this.Size = new System.Drawing.Size(500, 500);
            setControls();

            loadCommitFiles();
        }

        void loadCommitFiles()
        {
            Cursor.Current = Cursors.WaitCursor;

            string workTreePath = context.repoInfo.getWorkTreePath();

            fileList = new ArrayList();
            string[] _fileList = clsUtil.getAllFileList(workTreePath);
            foreach (string f in _fileList)
            {
                clsFile file = new clsFile();
                file.path = Path.GetDirectoryName(f).Substring(workTreePath.Length) + "/";
                file.path = file.path.Replace(@"\", "/");
                file.name = Path.GetFileName(f);
                file.size = new FileInfo(f).Length;

                fileList.Add(file);
            }

            setFileList("/");

            Cursor.Current = Cursors.Default;
        }

        void setControls()
        {
            ImageList imgList = new ImageList();
            imgList.Images.Add("DIR", Properties.Resources.ico_folder);
            imgList.Images.Add("FILE", Properties.Resources.ico_file);

            lvFile = new ListView();
            int margin = 20;
            //lvFile.Font = new System.Drawing.Font("돋음", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            lvFile.FullRowSelect = true;
            lvFile.Location = new System.Drawing.Point(margin, margin * 2);
            lvFile.Size = new System.Drawing.Size(this.Width - margin * 2, this.Height - margin * 3);
            lvFile.View = View.Details;
            lvFile.SmallImageList = imgList;
            lvFile.DoubleClick += new EventHandler(lvFile_DoubleClick);

            ColumnHeader h1 = new ColumnHeader();
            h1.Text = "이름";
            h1.Width = (int)(lvFile.Width * (2F / 3));
            lvFile.Columns.Add(h1);

            ColumnHeader h2 = new ColumnHeader();
            h2.Text = "크기";
            h2.Width = (int)(lvFile.Width * (0.9F / 3));
            lvFile.Columns.Add(h2);

            this.Controls.Add(lvFile);
        }

        void lvFile_DoubleClick(object sender, EventArgs e)
        {
            if (lvFile.SelectedItems.Count > 0)
            {
                if (lvFile.SelectedItems[0].ImageKey.Equals("DIR"))
                {
                    //Console.WriteLine(lvFile.SelectedItems.Count);

                    string path = curRootPath;

                    if (lvFile.SelectedItems[0].Text.Equals(".."))
                    {
                        //상위로
                        path = path.Substring(0, path.Length - 1);              //젤 끝에 / 제거
                        path = path.Substring(0, path.LastIndexOf("/") + 1);      //윗path

                    }
                    else
                    {
                        //하위 디렉토리
                        path += (lvFile.SelectedItems[0].Text + "/");
                    }

                    setFileList(path);
                }
                else  //파일 더블클릭
                {
                    string fileFullPath = context.repoInfo.getWorkTreePath() + curRootPath + lvFile.SelectedItems[0].Text;
                    Console.WriteLine(fileFullPath);

                    System.Diagnostics.Process.Start(fileFullPath);

                    this.Close();
                }
            }
            
        }

        string curRootPath = "";
        void setFileList(string rootPath)
        {
            Console.WriteLine("setFileList:\t" + rootPath);

            curRootPath = rootPath;

            lvFile.Items.Clear();

            //최상위가 아니면 윗단으로 갈수 있는 .. 추가
            if (!rootPath.Equals("/"))
            {
                //디렉토리
                ListViewItem item = new ListViewItem(new string[] { ".." });
                item.ImageKey = "DIR";
                lvFile.Items.Add(item);
            }

            foreach (clsFile f in fileList)
            {
                //rootPath 아랫단의 path
                int rootPos = f.path.IndexOf(rootPath);
                if (rootPos >= 0)
                {
                    string subPath = f.path.Substring(rootPos + rootPath.Length);
                    //Console.WriteLine(subPath + "\t" + f.name);

                    string[] pathSplit = subPath.Split(new char[] { '/' });
                    //Console.WriteLine(pathSplit.Length);

                    if (pathSplit.Length == 2)
                    {
                        
                        if (!lvFile.Items.ContainsKey(pathSplit[0]))
                        {
                            //디렉토리
                            lvFile.Items.Add(pathSplit[0], pathSplit[0], "DIR");
                        }

                        
                    }

                    if (f.path.Equals(rootPath))
                    {
                        //파일
                        ListViewItem item = new ListViewItem(new string[] { f.name, string.Format("{0:n0}", f.size) });
                        item.ImageKey = "FILE";
                        lvFile.Items.Add(item);
                    }
                }
            }
        }
    }
}
