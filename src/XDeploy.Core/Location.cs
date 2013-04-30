using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace XDeploy
{
    public class Location
    {
        public string Uri { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; }

        public Location()
        {
        }

        public Location(string uri)
            : this(uri, null, null)
        {
        }

        public Location(string uri, string userName, string password, string domain = null)
        {
            Require.NotNullOrEmpty(uri, "uri");

            Uri = uri;
            UserName = userName;
            Password = password;
            Domain = domain;
        }

        public Location(string uri, NetworkCredential credential)
        {
            Require.NotNullOrEmpty(uri, "uri");

            Uri = uri;

            if (credential != null)
            {
                UserName = credential.UserName;
                Password = credential.Password;
                Domain = credential.Domain;
            }
        }
    }
}
