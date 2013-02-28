
namespace ClingClientEngine {
    /// <summary>
    /// ClingRayEngine 초기화 시 사용되는 config.
    /// </summary>
    public class EngineConfig {
        /// <summary>
        /// REQUIRED
        /// 
        /// ClingRayEngine 내부에서 사용할 registry key 의 부모 key 이름을 지정한다.
        /// <see cref="RegistryHelper.ROOT_REGISTRY_KEY"/>
        /// <see cref="RegistryHelper.getSubRegistryPath()"/>
        /// </summary>
        public static string baseRegistryKeyName { get; set; }

        /// <summary>
        /// REQUIRED
        /// 
        /// Repository 들이 놓이게 될 container directory 의 경로를 지정한다.
        /// Repository 관련 동작 시 해당 디렉토리 밑의 경로를 참조한다.
        /// </summary>
        public static string repositoryContainerPath { get; set; }

        public static string authorName { get; set; }
        public static string authorEmail { get; set; }

        public static string sshIdentityPath { get; set; }
        public static string sshKnownHostsPath { get; set; }

        static EngineConfig()
        {
            // TODO 이 부분은 clingray 앱에서 사용하기 위해 default 로 설정하는 항목이므로
            // 추후 api 오픈 시에는 없애야 하는 항목입니다.
            baseRegistryKeyName = "ClingRay";       //default로 clingray 지정!

            RegistryHelper.instance.baseRegistryKeyName = baseRegistryKeyName;
            repositoryContainerPath = (string)RegistryHelper.instance.getValue(RegistryHelper.LOCAL_REPO_CONTAINER_PATH_REG_NAME);
        }

        public static bool isEmptyConfig()
        {
            return string.IsNullOrWhiteSpace(repositoryContainerPath);
        }
    }
}
