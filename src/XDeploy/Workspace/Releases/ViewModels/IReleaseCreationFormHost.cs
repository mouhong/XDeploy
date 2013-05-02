using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public interface IReleaseCreationFormHost
    {
        ShellViewModel Shell { get; }

        void OnReleaseCreated(CreateReleaseViewModel viewModel);

        void OnReleaseCreationCanceled(CreateReleaseViewModel viewModel);
    }
}
