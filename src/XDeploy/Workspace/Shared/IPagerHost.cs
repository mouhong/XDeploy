﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Shared
{
    public interface IPagerHost
    {
        IEnumerable<IResult> LoadPage(int pageIndex);
    }
}
