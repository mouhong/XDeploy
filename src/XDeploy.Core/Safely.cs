using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public static class Safely
    {
        static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void Execute(Action action, string logMessageForError = "Error executing action.")
        {
            Require.NotNull(action, "action");

            try
            {
                action();
            }
            catch (Exception ex)
            {
                _log.ErrorException(logMessageForError, ex);
            }
        }
    }
}
