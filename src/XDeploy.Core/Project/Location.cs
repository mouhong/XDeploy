using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Project
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
    }
}
