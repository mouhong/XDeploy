using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace
{
    public interface IProjectWorkContextAccessor
    {
        ProjectWorkContext GetCurrentWorkContext();

        void SetCurrentWorkContext(ProjectWorkContext context);
    }

    [Export(typeof(IProjectWorkContextAccessor))]
    public class ProjectWorkContextAccessor : IProjectWorkContextAccessor
    {
        public ProjectWorkContext GetCurrentWorkContext()
        {
            return ProjectWorkContext.Current;
        }

        public void SetCurrentWorkContext(ProjectWorkContext context)
        {
            ProjectWorkContext.SetCurrent(context);
        }
    }
}
