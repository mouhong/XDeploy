using Caliburn.Micro;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace
{
    public class DeploymentProjectViewModel : PropertyChangedBase
    {
        private string _name;
        private string _sourceDirectory;
        private DateTime? _lastReleaseCreatedAt;
        private int _totalDeployTargets;
        private int _totalReleases;

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

        public string SourceDirectory
        {
            get
            {
                return _sourceDirectory;
            }
            set
            {
                if (_sourceDirectory != value)
                {
                    _sourceDirectory = value;
                    NotifyOfPropertyChange(() => SourceDirectory);
                }
            }
        }

        public bool HasRelease
        {
            get
            {
                return TotalReleases > 0;
            }
        }

        public DateTime? LastReleaseCreatedAt
        {
            get
            {
                return _lastReleaseCreatedAt;
            }
            set
            {
                if (_lastReleaseCreatedAt != value)
                {
                    _lastReleaseCreatedAt = value;
                    NotifyOfPropertyChange(() => LastReleaseCreatedAt);
                }
            }
        }

        public int TotalDeployTargets
        {
            get
            {
                return _totalDeployTargets;
            }
            set
            {
                if (_totalDeployTargets != value)
                {
                    _totalDeployTargets = value;
                    NotifyOfPropertyChange(() => TotalDeployTargets);
                }
            }
        }

        public int TotalReleases
        {
            get
            {
                return _totalReleases;
            }
            set
            {
                if (_totalReleases != value)
                {
                    _totalReleases = value;
                    NotifyOfPropertyChange(() => TotalReleases);
                    NotifyOfPropertyChange(() => HasRelease);
                }
            }
        }

        public DeploymentProjectViewModel()
        {
        }

        public DeploymentProjectViewModel(DeploymentProject project)
        {
            UpdateFrom(project);
        }

        public void UpdateFrom(DeploymentProject project)
        {
            Name = project.Name;
            SourceDirectory = project.SourceDirectory;
            TotalReleases = project.TotalReleases;
            LastReleaseCreatedAt = project.LastReleaseCreatedAtUtc;
            TotalDeployTargets = project.TotalDeployTargets;
        }
    }
}
