using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Models
{
    public class ReleasePackagesViewModel : PageViewModelBase
    {
        public Workspace Workspace { get; private set; }

        public ReleasePackagesViewModel(Workspace workspace)
        {
            Title = "Releases";
            Workspace = workspace;
        }
    }
}
