using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ClingClient.controls;
using ClingClientEngine;
using ClingClient.subForms;
using System.Collections.Generic;
using ClingClient.Properties;
using ClingClient.utilities;

namespace ClingClient.forms
{
    public partial class subCommitTree : ClingControlBase, subFormInterface
    {
        ClingTree clingTree = null;
        ClingRepo repo = null;

        public subCommitTree(ClingRepo repo)
        {
            this.repo = repo;

            InitializeComponent();

            setControls();
            refreshProjectList();
        }

        public void refreshProjectList()
        {
            RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(repo);
            //context.getVersionCount
           
        }

        void setControls()
        {
            clingTree = new ClingTree();
            clingTree.Top = 50;
            clingTree.Left = 15;
            //lvProject.BackColor = Color.Red;


            this.Controls.Add(clingTree);
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
