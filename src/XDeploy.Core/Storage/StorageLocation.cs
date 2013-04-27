using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy.Storage
{
    public static class StorageLocation
    {
        public static IStorageLocation Create(string uri)
        {
            return Create(uri, null);
        }

        public static IStorageLocation Create(string uri, string userName, string password)
        {
            return Create(uri, new NetworkCredential(userName, password));
        }

        public static IStorageLocation Create(string uri, ICredentials credentials)
        {
            Require.NotNullOrEmpty(uri, "uri");

            IStorageLocation location = null;

            if (uri.StartsWith("ftp://", StringComparison.OrdinalIgnoreCase))
            {
                location = new FtpStorageLocation(uri);
            }
            else
            {
                location = new FileSystemStorageLocation(new DirectoryInfo(uri));
            }

            location.Credentials = credentials;

            return location;
        }
    }
}
