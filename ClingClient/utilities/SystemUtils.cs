using System;
using System.Linq;
using System.IO;
using System.Net.NetworkInformation;

namespace ClingClient.utilities {
    class SystemUtils {
        static public Int64 getFolderSize(string folderPath) {
            Int64 size = 0;

            try {
                var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories);
                size = (from file in files let fileInfo = new FileInfo(file) select fileInfo.Length).Sum();

            } catch (Exception e) {
                Console.WriteLine(e);
            }

            return size;
        }

        static public string getFileSizeAutoUnit(Int64 folderSize, int roundingIdx)
        {
            string[] UNITS = { "B", "K", "M", "G", "T" };
            int unitLevel = 0;

            Int64 sizeByUnit = folderSize;
            while ((sizeByUnit >> 10) != 0)
            { // divide by 1024.
                sizeByUnit >>= 10;
                unitLevel++;
            }

            double actualSizeByUnit = (double)folderSize / Math.Pow(1024, unitLevel);
            actualSizeByUnit = Math.Round(actualSizeByUnit, roundingIdx);
            return string.Format("{0:f" + roundingIdx + "}{1}", actualSizeByUnit, UNITS[unitLevel]);
        }

        static public string getFolderSizeAutoUnit(string stringFolderPath, int roundingIdx) {
            Int64 folderSize = getFolderSize(stringFolderPath);
            return getFileSizeAutoUnit(folderSize, roundingIdx);
        }

        static public bool isDirecotryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        /// <summary>
        /// Indicates whether any network connection is available
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool isNetworkAvailable()
        {
            return isNetworkAvailable(0);
        }

        /// <summary>
        /// Indicates whether any network connection is available.
        /// Filter connections below a specified speed, as well as virtual network cards.
        /// </summary>
        /// <param name="minimumSpeed">The minimum speed required. Passing 0 will not filter connection using speed.</param>
        /// <returns>
        ///     <c>true</c> if a network connection is available; otherwise, <c>false</c>.
        /// </returns>
        public static bool isNetworkAvailable(long minimumSpeed)
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return false;

            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // discard because of standard reasons
                if ((ni.OperationalStatus != OperationalStatus.Up) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                    (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
                    continue;

                // this allow to filter modems, serial, etc.
                // I use 10000000 as a minimum speed for most cases
                if (ni.Speed < minimumSpeed)
                    continue;

                // discard virtual cards (virtual box, virtual pc, etc.)
                if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                    continue;

                // discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
                if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
                    continue;

                return true;
            }
            return false;
        }
    }
}
