using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClingClientEngine
{
    public class Commit
    {
        public string hash { get; set; }
        public DateTime commitDate { get; set; }
        public string comment { get; set; }
        public string branchName { get; set; }
        public string repositoryUUID { get; set; }
        public string authorName { get; set; }
        public string authorEmail { get; set; }

        internal Commit()
        {
        }

        internal Commit(string hash, DateTime commitDate, string comment, string authorName, string authorEmail, string branchName, string repositoryUUID)
        {
            this.hash = hash;
            this.commitDate = commitDate;
            this.comment = comment;
            this.authorName = authorName;
            this.authorEmail = authorEmail;
            this.branchName = branchName;
            this.repositoryUUID = repositoryUUID;
        }

        public bool existCommitInArrayList(List<Commit> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Commit commit = list[i];
                if(commit.hash.Equals(this.hash)) return true;
            }
            return false;
        }
    }

    public class StructuralCommit : Commit
    {
        internal StructuralCommit()
        {
            children = new List<StructuralCommit>();
        }

        internal StructuralCommit(string hash, DateTime commitDate, string comment, string authorName, string authorEmail, string branchName, string repositoryUUID)
            : base(hash, commitDate, comment, authorName, authorEmail, branchName, repositoryUUID)
        {
            
        }

        public StructuralCommit findCommitByHash(string hash)
        {
            if (hash.Equals(this.hash))
            {
                return this;
            }

            foreach (StructuralCommit child in children)
            {
                StructuralCommit matching = child.findCommitByHash(hash);
                if (matching != null)
                {
                    return matching;
                }
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            fillTreeDescription(builder, 0);
            return builder.ToString();
        }

        private void fillTreeDescription(StringBuilder builder, int tabLevel)
        {
            for (int i = 0; i < tabLevel; i++)
            {
                builder.Append("\t");
            }
            builder.AppendLine(string.Format("{0}({1})", this.hash, this.branchName));
            foreach (StructuralCommit commit in children)
            {
                commit.fillTreeDescription(builder, tabLevel + 1);
            }
        }

        public List<StructuralCommit> children { get; internal set;  }
        public Commit parent { get; internal set; }
    }
}
