using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDeploy.Workspace;

namespace XDeploy
{
    public class AsyncActionResult : IResult
    {
        protected Action<AsyncActionContext> Action { get; private set; }
        protected Action<AsyncActionContext> ErrorAction { get; private set; }
        protected Action<AsyncActionContext> SuccessAction { get; private set; }

        public event EventHandler<ResultCompletionEventArgs> Completed = delegate { };

        public AsyncActionResult(Action<AsyncActionContext> action)
        {
            Action = action;
            WithDefaultExceptionHandler();
        }

        public AsyncActionResult OnSuccess(Action<AsyncActionContext> onSuccess)
        {
            SuccessAction = onSuccess;
            return this;
        }

        public AsyncActionResult OnError(Action<AsyncActionContext> onError)
        {
            ErrorAction = onError;
            return this;
        }

        public AsyncActionResult WithDefaultExceptionHandler()
        {
            WithExceptionHandler(ex =>
            {
                IoC.Get<IBusyIndicator>().Hide();
                IoC.Get<IMessageBox>().Error(ex);
            });
            return this;
        }

        public AsyncActionResult WithExceptionHandler(Action<Exception> handler)
        {
            ErrorAction = context =>
            {
                context.HandleException(handler);
            };
            return this;
        }

        public void Execute(ActionExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                var actionContext = new AsyncActionContext(this, context);

                try
                {
                    Action(actionContext);
                    Caliburn.Micro.Execute.OnUIThread(() => OnExecutionSuccess(actionContext));
                }
                catch (Exception ex)
                {
                    actionContext.SetException(ex);

                    Caliburn.Micro.Execute.OnUIThread(() =>
                    {
                        OnExecutionError(actionContext);

                        if (!actionContext.IsExceptionHandled)
                        {
                            throw ex;
                        }
                    });
                }
            });
        }

        protected virtual void OnExecutionSuccess(AsyncActionContext context)
        {
            if (SuccessAction != null)
            {
                SuccessAction(context);
            }

            Completed(this, new ResultCompletionEventArgs());
        }

        protected virtual void OnExecutionError(AsyncActionContext context)
        {
            if (ErrorAction != null)
            {
                ErrorAction(context);
            }

            Completed(this, new ResultCompletionEventArgs
            {
                Error = context.Exception,
                WasCancelled = false
            });
        }
    }
}
