using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class Require
    {
        public static void NotNull(object param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        public static void NotNullOrEmpty(string param, string paramName)
        {
            if (String.IsNullOrEmpty(param))
                throw new ArgumentException("\"" + paramName + "\" is required.", paramName);
        }

        public static void That(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}
