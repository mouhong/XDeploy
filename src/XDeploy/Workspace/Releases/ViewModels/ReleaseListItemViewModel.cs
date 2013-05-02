using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Releases.ViewModels
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
                }
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
                }
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
