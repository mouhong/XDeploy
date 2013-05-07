using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy
{
    public class BackupFolderNameTemplateModel
    {
        public int ReleaseId { get; set; }

        public string ReleaseName { get; set; }

        public string DateTime { get; set; }

        public void SetDateTime(DateTime datetime)
        {
            DateTime = datetime.ToString("yyyyMMddHHmmss");
        }
    }
}
