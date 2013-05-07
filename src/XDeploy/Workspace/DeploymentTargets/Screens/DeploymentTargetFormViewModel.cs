using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shared;
using XDeploy.Wpf.Framework.Validation;

namespace XDeploy.Workspace.DeploymentTargets.Screens
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

        private LocationFormViewModel _backupRootLocation;

        public LocationFormViewModel BackupRootLocation
        {
            get
            {
                return _backupRootLocation;
            }
            set
            {
                if (_backupRootLocation != value)
                {
                    _backupRootLocation = value;
                    NotifyOfPropertyChange(() => BackupRootLocation);
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

        public DeploymentTargetFormViewModel()
        {
            _deployLocation = new LocationFormViewModel();
            _backupRootLocation = new LocationFormViewModel
            {
                UriLabel = "Root Uri:"
            };
            _backupFolderNameTemplate = DeploymentTarget.DefaultBackupFolderNameTemplate;
        }

        public void UpdateFrom(DeploymentTarget target)
        {
            Require.NotNull(target, "target");

            Id = target.Id;
            TargetName = target.Name;
            DeployLocation.UpdateFrom(target.DeployLocation);

            if (target.BackupRootLocation != null)
            {
                BackupRootLocation.UpdateFrom(target.BackupRootLocation);
            }
            else
            {
                BackupRootLocation.Reset();
            }

            BackupFolderNameTemplate = target.BackupFolderNameTemplate;
        }

        public void UpdateTo(DeploymentTarget target)
        {
            Require.NotNull(target, "target");

            target.Name = TargetName.Trim();
            DeployLocation.UpdateTo(target.DeployLocation);

            if (BackupRootLocation != null)
            {
                if (target.BackupRootLocation == null)
                {
                    target.BackupRootLocation = new Location();
                }

                BackupRootLocation.UpdateTo(target.BackupRootLocation);
            }
            else
            {
                target.BackupRootLocation = null;
            }

            target.BackupFolderNameTemplate = BackupFolderNameTemplate;
        }
    }
}
