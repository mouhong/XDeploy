using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace XDeploy.Workspace
{
    [Export(typeof(IBusyIndicator))]
    public class DefaultBusyIndicator : PropertyChangedBase, IBusyIndicator
    {
        private bool _isBusy;

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                if (_isBusy != value)
                {
                    _isBusy = value;
                    NotifyOfPropertyChange(() => IsBusy);
                }
            }
        }

        private string _message;

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    NotifyOfPropertyChange(() => Message);
                }
            }
        }
    }
}
