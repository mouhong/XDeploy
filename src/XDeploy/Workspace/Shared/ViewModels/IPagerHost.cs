using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Shared.ViewModels
{
    public interface IPagerHost
    {
        IEnumerable<IResult> LoadPage(int pageIndex);
    }
}
