using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class AsyncActionContext
    {
        public ActionExecutionContext ExecutionContext { get; private set; }

        public AsyncActionResult Result { get; private set; }

        public Exception Exception { get; private set; }

        public bool IsExceptionHandled { get; set; }

        public AsyncActionContext(AsyncActionResult result, ActionExecutionContext executionContext, Exception ex = null)
        {
            Result = result;
            ExecutionContext = executionContext;
            Exception = ex;
        }

        public void SetException(Exception ex)
        {
            Exception = ex;
        }

        public void HandleException(Action<Exception> handler)
        {
            handler(Exception);
            IsExceptionHandled = true;
        }
    }
}
