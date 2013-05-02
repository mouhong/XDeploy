﻿using Caliburn.Micro;
using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Home.ViewModels
{
    public class WelcomeScreenViewModel : Screen
    {
        public ShellViewModel Shell { get; private set; }

        public WelcomeScreenViewModel(ShellViewModel shell)
        {
            Shell = shell;
        }

        public void CreateProject()
        {
            Shell.CreateProject();
        }

        public IEnumerable<IResult> OpenProject()
        {
            return Shell.OpenProject();
        }
    }
}
