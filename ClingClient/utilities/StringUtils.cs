using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClingClient.utilities
{
    public class StringUtils
    {
        public const string ELLIPSIS = "...";
        public static string getStringWithEllipsis(string text, int maxLength)
        {
            string result = text;

            if (text.Length > maxLength)
            {
                result = result.Substring(0, Math.Min(maxLength - ELLIPSIS.Length, result.Length));
                result += ELLIPSIS;
            }

            return result;
        }
    }
}
