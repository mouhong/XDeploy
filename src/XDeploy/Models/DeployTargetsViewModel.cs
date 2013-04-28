using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Models
{
    public class DeployTargetsViewModel : PageViewModelBase
    {
        public Workspace Workspace { get; private set; }

        public DeployTargetsViewModel(Workspace workspace)
        {
            Title = "Deploy Targets";
            Workspace = workspace;
        }
    }
}
