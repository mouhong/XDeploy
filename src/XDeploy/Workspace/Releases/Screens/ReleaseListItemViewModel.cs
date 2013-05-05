using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Releases.Screens
{
    public class ReleaseListItemViewModel : PropertyChangedBase
    {
        public int Id { get; set; }

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyOfPropertyChange(() => Name);
                    NotifyOfPropertyChange(() => ItemTitle);
                }
            }
        }

        public string ItemTitle
        {
            get
            {
                return "Release - " + Name;
            }
        }

        private string _releaseNotes;

        public string ReleaseNotes
        {
            get
            {
                return _releaseNotes;
            }
            set
            {
                if (_releaseNotes != value)
                {
                    _releaseNotes = value;
                    NotifyOfPropertyChange(() => ReleaseNotes);
                    NotifyOfPropertyChange(() => HasReleaseNotes);
                }
            }
        }

        public bool HasReleaseNotes
        {
            get
            {
                return !String.IsNullOrWhiteSpace(ReleaseNotes);
            }
        }

        public DateTime CreatedAt { get; set; }

        public ReleaseListItemViewModel()
        {
        }

        public ReleaseListItemViewModel(Release release)
        {
            UpdateFrom(release);
        }

        public void UpdateFrom(Release release)
        {
            Id = release.Id;
            Name = release.Name;
            ReleaseNotes = release.ReleaseNotes;
            CreatedAt = release.CreatedAtUtc.ToLocalTime();
        }
    }
}
