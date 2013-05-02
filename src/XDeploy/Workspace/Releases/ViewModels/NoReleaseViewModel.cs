using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class NoReleaseViewModel : Screen
    {
        public ShellViewModel Shell
        {
            get
            {
                return Host.Shell;
            }
        }

        public ProjectReleasesViewModel Host { get; private set; }

        public NoReleaseViewModel(ProjectReleasesViewModel host)
        {
            Host = host;
        }

        public void CreateNewRelease()
        {
            Host.CreateRelease();
        }
    }
}
