using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Shared
{
    public class LocationFormViewModel : PropertyChangedBase
    {
        private string _uri;

        public string Uri
        {
            get
            {
                return _uri;
            }
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    NotifyOfPropertyChange(() => Uri);
                }
            }
        }

        private string _userName;

        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyOfPropertyChange(() => UserName);
                }
            }
        }

        private string _password;

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyOfPropertyChange(() => Password);
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(Uri) && String.IsNullOrEmpty(UserName) && String.IsNullOrEmpty(Password);
            }
        }

        public void Reset()
        {
            Uri = null;
            UserName = null;
            Password = null;
        }

        public void UpdateFrom(Location location)
        {
            Uri = location.Uri;
            UserName = location.UserName;
            Password = location.Password;
        }

        public void UpdateTo(Location location)
        {
            location.Uri = Uri.TrimIfNotNull();
            location.UserName = UserName.TrimIfNotNull();
            location.Password = Password;
        }
    }
}
