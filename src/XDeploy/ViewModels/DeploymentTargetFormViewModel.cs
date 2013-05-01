using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class DeploymentTargetFormViewModel : PropertyChangedBase
    {
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

        public void UpdateTo(DeploymentTarget target)
        {
            target.Name = Name.Trim();
            DeployLocation.UpdateTo(target.DeployLocation);
            BackupLocation.UpdateTo(target.BackupLocation);
        }
    }
}
