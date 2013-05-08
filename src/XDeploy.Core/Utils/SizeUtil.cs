using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Utils
{
    public static class SizeUtil
    {
        static readonly string[] _sizeUnits = { "", "K", "M", "G", "T" };

        public static string GetFriendlyDisplay(long bytes, string format = "{0} {1}")
        {
            double value = bytes;

            for (var i = 0; i < _sizeUnits.Length; i++)
            {
                if (i > 0)
                {
                    value = Math.Round(value / 1024, 1);
                }

                if (value < 1024)
                {
                    return String.Format(format, value, _sizeUnits[i] + "B");
                }
            }

            return String.Format(format, value, _sizeUnits[_sizeUnits.Length - 1]);
        }
    }
}
