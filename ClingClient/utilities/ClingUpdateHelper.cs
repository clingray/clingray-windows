using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClingUpdater;

namespace ClingClient.updater
{
    public sealed class ClingUpdateHelper
    {
        private const string UPDATE_PACKAGE_NAME = "clingray";

        public static UpdateChecker getUpdateChecker()
        {
            UpdateChecker.DistributionType distribution;
            UpdateChecker checker;

            if (AppConfig.isInternal())
            {
                distribution = UpdateChecker.DistributionType.DEVELOPMENT;
            }
            else
            {
                distribution = UpdateChecker.DistributionType.PRODUCTION;
            }

            checker = new UpdateChecker(UPDATE_PACKAGE_NAME, distributionType: distribution);

            return checker;
        }
    }
}
