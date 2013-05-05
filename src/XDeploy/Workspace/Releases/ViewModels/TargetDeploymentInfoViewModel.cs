using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class TargetDeploymentInfoViewModel : PropertyChangedBase
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
                }
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
                }
            }
        }

        public TargetDeploymentInfoViewModel()
        {
        }
    }
}
