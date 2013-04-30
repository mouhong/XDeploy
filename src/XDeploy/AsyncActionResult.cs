using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XDeploy
{
    public class AsyncActionResult : ActionResult
    {
        public AsyncActionResult(Action<ActionContext> action)
            : base(action)
        {
        }

        public override void Execute(ActionExecutionContext context)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Action(new ActionContext(this, context));
                    Caliburn.Micro.Execute.OnUIThread(() => OnExecutionSuccess(context));
                }
                catch (Exception ex)
                {
                    Caliburn.Micro.Execute.OnUIThread(() => OnExecutionError(context, ex));
                }
            });
        }
    }
}
