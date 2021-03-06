﻿using System;
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

        public static string Quote(this string str, string open = "\"", string close = "\"")
        {
            return open + str + close;
        }

        public static string Shorten(this string str, int maxchars, string more = "...")
        {
            if (String.IsNullOrEmpty(str) || str.Length <= maxchars)
            {
                return str;
            }

            return str.Substring(0, maxchars) + more;
        }
    }
}
