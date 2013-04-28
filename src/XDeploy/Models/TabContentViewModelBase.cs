using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace XDeploy.Models
{
    public class TabContentViewModelBase : ViewModelBase
    {
        private Visibility _visiblity;

        public Visibility Visibility
        {
            get
            {
                return _visiblity;
            }
            set
            {
                if (value != _visiblity)
                {
                    _visiblity = value;
                    OnPropertyChanged("Visibility");
                }
            }
        }
    }
}
