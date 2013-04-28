using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.Models
{
    public class PageViewModelBase : ViewModelBase
    {
        public string Title { get; set; }

        private bool _isSelected;

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    OnPropertyChanged("IsSelected");
                }
            }
        }

        public virtual void OnProjectLoaded(DeploymentProjectViewModel project)
        {
        }
    }
}
