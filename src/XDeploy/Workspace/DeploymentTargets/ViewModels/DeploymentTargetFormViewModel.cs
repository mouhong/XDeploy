using Caliburn.Micro;
using Caliburn.Micro.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shared.ViewModels;

namespace XDeploy.Workspace.DeploymentTargets.ViewModels
{
    public class DeploymentTargetFormViewModel : ValidatablePropertyChangedBase
    {
        public int Id { get; set; }

        private string _targetName;

        [Required]
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

        private LocationFormViewModel _deployLocation;

        public LocationFormViewModel DeployLocation
        {
            get
            {
                return _deployLocation;
            }
            set
            {
                if (_deployLocation != value)
                {
                    _deployLocation = value;
                    NotifyOfPropertyChange(() => DeployLocation);
                }
            }
        }

        private LocationFormViewModel _backupLocation;

        public LocationFormViewModel BackupLocation
        {
            get
            {
                return _backupLocation;
            }
            set
            {
                if (_backupLocation != value)
                {
                    _backupLocation = value;
                    NotifyOfPropertyChange(() => BackupLocation);
                }
            }
        }

        public DeploymentTargetFormViewModel()
        {
            _deployLocation = new LocationFormViewModel();
            _backupLocation = new LocationFormViewModel();
        }

        public void Reset()
        {
            TargetName = null;
            if (DeployLocation != null)
            {
                DeployLocation.Reset();
            }
            if (BackupLocation != null)
            {
                BackupLocation.Reset();
            }
        }

        public void UpdateFrom(DeploymentTarget target)
        {
            Id = target.Id;
            TargetName = target.Name;
            DeployLocation.UpdateFrom(target.DeployLocation);

            if (target.BackupLocation != null)
            {
                BackupLocation.UpdateFrom(target.BackupLocation);
            }
            else
            {
                BackupLocation.Reset();
            }
        }

        public void UpdateTo(DeploymentTarget target)
        {
            target.Name = TargetName.Trim();
            DeployLocation.UpdateTo(target.DeployLocation);

            if (BackupLocation != null)
            {
                if (target.BackupLocation == null)
                {
                    target.BackupLocation = new Location();
                }

                BackupLocation.UpdateTo(target.BackupLocation);
            }
            else
            {
                target.BackupLocation = null;
            }
        }
    }
}
