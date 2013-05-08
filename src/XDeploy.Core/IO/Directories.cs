using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using XDeploy.IO.Ftp;
using XDeploy.IO.Local;

namespace XDeploy.IO
{
    public static class Directories
    {
        public static IDirectory GetDirectory(string uri)
        {
            return GetDirectory(uri, null);
        }

        public static IDirectory GetDirectory(string uri, string userName, string password)
        {
            NetworkCredential credential = null;

            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {
                credential = new NetworkCredential(userName, password);
            }

            return GetDirectory(uri, credential);
        }

        public static IDirectory GetDirectory(string uri, NetworkCredential credential)
        {
            Require.NotNullOrEmpty(uri, "uri");

            IDirectory directory = null;

            if (uri.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                directory = new FtpDirectory("/", new Uri(uri), credential);
            }
            else
            {
                directory = new LocalDirectory("/", new DirectoryInfo(uri));
            }

            return directory;
        }
    }
}
