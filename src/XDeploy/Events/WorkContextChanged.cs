using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.ViewModels;

namespace XDeploy.Events
{
    public class WorkContextChanged
    {
        public WorkContext NewWorkContext { get; private set; }

        public WorkContextChanged(WorkContext newWorkContext)
        {
            NewWorkContext = newWorkContext;
        }
    }
}
