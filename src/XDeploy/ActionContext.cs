using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ActionContext
    {
        public ActionExecutionContext ExecutionContext { get; private set; }

        public ActionResult Result { get; private set; }

        public Exception Exception { get; private set; }

        public bool IsExceptionHandled { get; set; }

        public ActionContext(ActionResult result, ActionExecutionContext executionContext, Exception ex = null)
        {
            Result = result;
            ExecutionContext = executionContext;
            Exception = ex;
        }
    }
}
