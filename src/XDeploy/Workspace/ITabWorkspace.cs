using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell;

namespace XDeploy.Workspace
{
    public interface ITabWorkspace : IConductor
    {
        string DisplayName { get; }

        int DisplayOrder { get; }

        bool IsVisible { get; set; }
    }

    public static class WorkspaceExtensions
    {
        public static ShellViewModel GetShell(this ITabWorkspace workspace)
        {
            var screen = (Screen)workspace;
            return (ShellViewModel)screen.Parent;
        }
    }

    public interface ITabContentScreen
    {
    }

    public static class WorkspaceScreenExtensions
    {
        public static ITabWorkspace GetWorkspace(this ITabContentScreen workspaceScreen)
        {
            var screen = (Screen)workspaceScreen;
            return (ITabWorkspace)screen.Parent;
        }
    }
}
