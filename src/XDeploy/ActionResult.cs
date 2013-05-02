using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class ActionResult : IResult
    {
        protected Action<ActionContext> Action { get; private set; }
        protected Action<ActionContext> ErrorAction { get; private set; }
        protected Action<ActionContext> SuccessAction { get; private set; }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        public ActionResult(Action<ActionContext> action)
        {
            Action = action;
        }

        public ActionResult OnSuccess(Action<ActionContext> onSuccess)
        {
            SuccessAction = onSuccess;
            return this;
        }

        public ActionResult OnError(Action<ActionContext> onError)
        {
            ErrorAction = onError;
            return this;
        }

        public virtual void Execute(ActionExecutionContext context)
        {
            try
            {
                Action(new ActionContext(this, context));
                OnExecutionSuccess(context);
            }
            catch (Exception ex)
            {
                var actionContext = OnExecutionError(context, ex);
                if (actionContext == null || !actionContext.IsExceptionHandled)
                {
                    throw;
                }
            }
        }

        protected virtual ActionContext OnExecutionSuccess(ActionExecutionContext context)
        {
            var actionContext = new ActionContext(this, context);

            if (SuccessAction != null)
            {
                SuccessAction(actionContext);
            }

            Completed(this, new ResultCompletionEventArgs());

            return actionContext;
        }

        protected virtual ActionContext OnExecutionError(ActionExecutionContext context, Exception error)
        {
            var actionContext = new ActionContext(this, context, error);

            if (ErrorAction != null)
            {
                ErrorAction(actionContext);
            }

            Completed(this, new ResultCompletionEventArgs
            {
                Error = error,
                WasCancelled = false
            });

            return actionContext;
        }
    }
}
