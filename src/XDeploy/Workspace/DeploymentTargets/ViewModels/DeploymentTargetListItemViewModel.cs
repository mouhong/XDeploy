using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class DeploymentTargetListItemViewModel : PropertyChangedBase
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

        private string _deployLocationUri;

        public string DeployLocationUri
        {
            get
            {
                return _deployLocationUri;
            }
            set
            {
                if (_deployLocationUri != value)
                {
                    _deployLocationUri = value;
                    NotifyOfPropertyChange(() => DeployLocationUri);
                }
            }
        }

        private string _backupLocationUri;

        public string BackupLocationUri
        {
            get
            {
                return _backupLocationUri;
            }
            set
            {
                if (_backupLocationUri != value)
                {
                    _backupLocationUri = value;
                    NotifyOfPropertyChange(() => BackupLocationUri);
                }
            }
        }

        private DateTime? _lastDeployedAt;

        public DateTime? LastDeployedAt
        {
            get
            {
                return _lastDeployedAt;
            }
            set
            {
                if (_lastDeployedAt != value)
                {
                    _lastDeployedAt = value;
                    NotifyOfPropertyChange(() => LastDeployedAt);
                }
            }
        }

        private DateTime? _lastBackuppedAt;

        public DateTime? LastBackuppedAt
        {
            get
            {
                return _lastBackuppedAt;
            }
            set
            {
                if (_lastBackuppedAt != value)
                {
                    _lastBackuppedAt = value;
                    NotifyOfPropertyChange(() => LastBackuppedAt);
                }
            }
        }

        public DateTime CreatedAt { get; set; }

        public DeploymentTargetListItemViewModel()
        {
        }

        public DeploymentTargetListItemViewModel(DeploymentTarget target)
        {
            Id = target.Id;
            Name = target.Name;
            DeployLocationUri = target.DeployLocation.Uri;
            BackupLocationUri = target.BackupLocation == null ? null : target.BackupLocation.Uri;
            LastDeployedAt = target.LastDeployedAtUtc == null ? null : (DateTime?)target.LastDeployedAtUtc.Value.ToLocalTime();
            LastBackuppedAt = target.LastBackuppedAtUtc == null ? null : (DateTime?)target.LastBackuppedAtUtc.Value.ToLocalTime();
            CreatedAt = target.CreatedAtUtc.ToLocalTime();
        }
    }
}
