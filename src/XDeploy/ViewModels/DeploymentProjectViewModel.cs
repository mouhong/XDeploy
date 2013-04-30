using Caliburn.Micro;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class DeploymentProjectViewModel : PropertyChangedBase
    {
        private string _name;
        private string _sourceDirectory;
        private DateTime? _lastReleaseCreationTime;
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

        public DateTime? LastReleaseCreationTime
        {
            get
            {
                return _lastReleaseCreationTime;
            }
            set
            {
                if (_lastReleaseCreationTime != value)
                {
                    _lastReleaseCreationTime = value;
                    NotifyOfPropertyChange(() => LastReleaseCreationTime);
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
                }
            }
        }

        public DeploymentProjectViewModel()
        {
        }

        public static DeploymentProjectViewModel Create(WorkContext workContext)
        {
            var project = workContext.Project;

            var model = new DeploymentProjectViewModel
            {
                Name = project.Name,
                SourceDirectory = project.SourceDirectory
            };

            using (var session = workContext.Database.OpenSession())
            {
                var lastRelease = session.Query<Release>().OrderByDescending(x => x.Id).FirstOrDefault();
                model.LastReleaseCreationTime = lastRelease == null ? null : (DateTime?)lastRelease.CreatedAtUtc.ToLocalTime();
                model.TotalDeployTargets = session.Query<DeployTarget>().Count();
            }

            return model;
        }
    }
}
