using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public enum ProcessingStatus
    {
        Pending = 0,
        InProgress = 1,
        Succeeded = 2,
        Failed = 3,
        Ignored = 4
    }
}
