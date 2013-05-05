using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace XDeploy
{
    [Export(typeof(StartupArguments))]
    public class StartupArguments
    {
        public string Path { get; private set; }

        public void SetPath(string path)
        {
            Path = path;
        }
    }
}
