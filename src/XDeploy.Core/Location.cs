using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using XDeploy.IO;

namespace XDeploy
{
    public class Location
    {
        public string Uri { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public Location()
        {
        }

        public Location(string uri)
            : this(uri, null, null)
        {
        }

        public Location(string uri, string userName, string password)
        {
            Require.NotNullOrEmpty(uri, "uri");

            Uri = uri;
            UserName = userName;
            Password = password;
        }

        public Location(string uri, NetworkCredential credential)
        {
            Require.NotNullOrEmpty(uri, "uri");

            Uri = uri;

            if (credential != null)
            {
                UserName = credential.UserName;
                Password = credential.Password;
            }
        }

        public virtual bool IsEmpty()
        {
            return String.IsNullOrWhiteSpace(Uri)
                && String.IsNullOrWhiteSpace(UserName)
                && String.IsNullOrWhiteSpace(Password);
        }

        public virtual IDirectory GetDirectory()
        {
            return Directories.GetDirectory(Uri, UserName, Password);
        }
    }
}
