using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Releases.Screens
{
    public class AvailableTargetViewModel : PropertyChangedBase
    {
        private int _targetId;

        public int TargetId
        {
            get
            {
                return _targetId;
            }
            set
            {
                if (_targetId != value)
                {
                    _targetId = value;
                    NotifyOfPropertyChange(() => TargetId);
                }
            }
        }

        private string _targetName;

        public string TargetName
        {
            get
            {
                return _targetName;
            }
            set
            {
                if (_targetName != value)
                {
                    _targetName = value;
                    NotifyOfPropertyChange(() => TargetName);
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

        private bool _isDeployed;

        public bool IsDeployed
        {
            get
            {
                return _isDeployed;
            }
            set
            {
                if (_isDeployed != value)
                {
                    _isDeployed = value;
                    NotifyOfPropertyChange(() => IsDeployed);
                    NotifyOfPropertyChange(() => DeployedTooltip);
                }
            }
        }

        private DateTime? _deployedAt;

        public DateTime? DeployedAt
        {
            get
            {
                return _deployedAt;
            }
            set
            {
                if (_deployedAt != value)
                {
                    _deployedAt = value;
                    NotifyOfPropertyChange(() => DeployedAt);
                    NotifyOfPropertyChange(() => DeployedTooltip);
                }
            }
        }

        public string DeployedTooltip
        {
            get
            {
                if (IsDeployed)
                {
                    return "Deployed at " + DeployedAt;
                }

                return null;
            }
        }

        private bool _hasSetBackupLocation;

        public bool HasSetBackupLocation
        {
            get
            {
                return _hasSetBackupLocation;
            }
            set
            {
                if (_hasSetBackupLocation != value)
                {
                    _hasSetBackupLocation = value;
                    NotifyOfPropertyChange(() => HasSetBackupLocation);
                }
            }
        }

        private string _backupFolderNameTemplate;

        public string BackupFolderNameTemplate
        {
            get
            {
                return _backupFolderNameTemplate;
            }
            set
            {
                if (_backupFolderNameTemplate != value)
                {
                    _backupFolderNameTemplate = value;
                    NotifyOfPropertyChange(() => BackupFolderNameTemplate);
                }
            }
        }

        public AvailableTargetViewModel()
        {
        }
    }
}
