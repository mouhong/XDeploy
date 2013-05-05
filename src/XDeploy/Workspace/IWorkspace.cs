using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace
{
    public interface IWorkspace : IConductor
    {
        string DisplayName { get; }

        bool IsVisible { get; set; }
    }

    public static class WorkspaceExtensions
    {
        public static ShellViewModel GetShell(this IWorkspace workspace)
        {
            var screen = (Screen)workspace;
            return (ShellViewModel)screen.Parent;
        }
    }

    public interface IWorkspaceScreen
    {
    }

    public static class WorkspaceScreenExtensions
    {
        public static IWorkspace GetWorkspace(this IWorkspaceScreen workspaceScreen)
        {
            var screen = (Screen)workspaceScreen;
            return (IWorkspace)screen.Parent;
        }
    }
}
