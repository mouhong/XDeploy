using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using XDeploy.Wpf;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class FileViewModel : PropertyChangedBase
    {
        private string _virtualPath;

        public string VirtualPath
        {
            get
            {
                return _virtualPath;
            }
            set
            {
                if (_virtualPath != value)
                {
                    _virtualPath = value;
                    NotifyOfPropertyChange(() => VirtualPath);
                }
            }
        }

        private ProcessingStatus _backupStatus;

        public ProcessingStatus BackupStatus
        {
            get
            {
                return _backupStatus;
            }
            set
            {
                if (_backupStatus != value)
                {
                    _backupStatus = value;
                    NotifyOfPropertyChange(() => BackupStatus);
                    NotifyOfPropertyChange(() => HasBackupErrors);
                    NotifyOfPropertyChange(() => BackupStatusTextBrush);
                }
            }
        }

        public bool HasBackupErrors
        {
            get
            {
                return BackupStatus == ProcessingStatus.Failed;
            }
        }

        private string _backupErrorMessage;

        public string BackupErrorMessage
        {
            get
            {
                return _backupErrorMessage;
            }
            set
            {
                if (_backupErrorMessage != value)
                {
                    _backupErrorMessage = value;
                    NotifyOfPropertyChange(() => BackupErrorMessage);
                }
            }
        }

        private string _backupErrorDetail;

        public string BackupErrorDetail
        {
            get
            {
                return _backupErrorDetail;
            }
            set
            {
                if (_backupErrorDetail != value)
                {
                    _backupErrorDetail = value;
                    NotifyOfPropertyChange(() => BackupErrorDetail);
                }
            }
        }

        private ProcessingStatus _deployStatus;

        public ProcessingStatus DeployStatus
        {
            get
            {
                return _deployStatus;
            }
            set
            {
                if (_deployStatus != value)
                {
                    _deployStatus = value;
                    NotifyOfPropertyChange(() => DeployStatus);
                    NotifyOfPropertyChange(() => HasDeployErrors);
                    NotifyOfPropertyChange(() => DeployStatusTextBrush);
                }
            }
        }

        public bool HasDeployErrors
        {
            get
            {
                return DeployStatus == ProcessingStatus.Failed;
            }
        }

        private string _deployErrorMessage;

        public string DeployErrorMessage
        {
            get
            {
                return _deployErrorMessage;
            }
            set
            {
                if (_deployErrorMessage != value)
                {
                    _deployErrorMessage = value;
                    NotifyOfPropertyChange(() => DeployErrorMessage);
                }
            }
        }

        private string _deployErrorDetail;

        public string DeployErrorDetail
        {
            get
            {
                return _deployErrorDetail;
            }
            set
            {
                if (_deployErrorDetail != value)
                {
                    _deployErrorDetail = value;
                    NotifyOfPropertyChange(() => DeployErrorDetail);
                }
            }
        }

        public Brush BackupStatusTextBrush
        {
            get
            {
                return GetBrush(BackupStatus);
            }
        }

        public Brush DeployStatusTextBrush
        {
            get
            {
                return GetBrush(BackupStatus);
            }
        }

        public Brush GetBrush(ProcessingStatus status)
        {
            if (status == ProcessingStatus.Pending)
            {
                return Brushes.Gray;
            }
            if (status == ProcessingStatus.InProgress)
            {
                return Brushes.Green;
            }
            if (status == ProcessingStatus.Succeeded)
            {
                return Brushes.Green;
            }

            return Brushes.Red;
        }
    }
}
