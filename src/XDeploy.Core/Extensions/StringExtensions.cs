using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class StringExtensions
    {
        public static string TrimIfNotNull(this string str)
        {
            return str == null ? null : str.Trim();
        }
    }
}
