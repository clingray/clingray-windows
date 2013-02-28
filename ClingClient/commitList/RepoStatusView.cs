using ClingClientEngine;
using ClingClient.Properties;

namespace ClingClient.controls {
    public partial class RepoStatusView : ClingControlBase
    {
        public RepositoryContext.Status repositoryStatus { get; set; } 
        public RepoStatusView() {
            InitializeComponent();
        }

        public void setRepositoryStatus(RepositoryContext.Status repositoryStatus) 
        {
            //관리상태 버튼
            if (repositoryStatus == RepositoryContext.Status.UNKNOWN) {
                localStatusButton.Enabled = false;
                localStatusButton.bmpNormal = Resources.ico_state_record_disable;
                repoStatusLabel.Image = Resources.txt_state_record_disable;
            }
            else if (repositoryStatus.HasFlag(RepositoryContext.Status.UNLINKED))
            {
                localStatusButton.Enabled = true;
                localStatusButton.bmpNormal = Resources.ico_state_record_off;
                repoStatusLabel.Image = Resources.txt_state_record_off;
            } 
            else if(repositoryStatus.HasFlag(RepositoryContext.Status.CHANGED) 
                || repositoryStatus.HasFlag(RepositoryContext.Status.LINKED))
            {
                localStatusButton.Enabled = false;
                localStatusButton.bmpNormal = Resources.ico_state_record_on;
                repoStatusLabel.Image = Resources.txt_state_record_on;
            }
        }
    }
}
