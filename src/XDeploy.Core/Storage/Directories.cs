using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.Storage
{
    public static class Directories
    {
        public static IDirectory GetDirectory(string uri)
        {
            return GetDirectory(uri, null);
        }

        public static IDirectory GetDirectory(string uri, string userName, string password)
        {
            ICredentials credentials = null;

            if (!String.IsNullOrEmpty(userName) && !String.IsNullOrEmpty(password))
            {
                credentials = new NetworkCredential(userName, password);
            }

            return GetDirectory(uri, credentials);
        }

        public static IDirectory GetDirectory(string uri, ICredentials credentials)
        {
            Require.NotNullOrEmpty(uri, "uri");

            IDirectory location = null;

            if (uri.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                location = new FtpDirectory(uri);
            }
            else
            {
                location = new FileSystemDirectory(new DirectoryInfo(uri));
            }

            location.Credentials = credentials;

            return location;
        }
    }
}
