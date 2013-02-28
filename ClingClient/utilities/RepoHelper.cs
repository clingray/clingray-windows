using ClingClientEngine;
using ClingClient.forms;
using ClingClient.Properties;

namespace ClingClient.utilities
{
    class RepoHelper
    {
        public RepositoryContext.Status getTotalRepoStatus(ClingRepo repo)
        {
            RepositoryContext context = ClingRayEngine.instance.getRepositoryContext(repo);
            RepositoryContext.Status repoStatus = context.getStatus();      //여기까지는 local status만 가지고 있음!

            return repoStatus;
        }

        public static int makeCoverImageIndex()
        {
            //int coverImageIndex = (new Random()).Next(subProjectList.CDJacketList.Length);

            //순차적으로 로테이션 되는 방식으로 변경!
            int coverImageIndex = Settings.Default.coverImageIndex++;
            if (coverImageIndex >= subProjectList.CDJacketForProjectList.Length)
            {
                Settings.Default.coverImageIndex = 0;
                coverImageIndex = Settings.Default.coverImageIndex++;
            }
            Settings.Default.Save();

            return coverImageIndex;
        }
    }
}
