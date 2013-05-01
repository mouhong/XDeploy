using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XDeploy.ViewModels
{
    public class BusyIndicatorViewModel : PropertyChangedBase
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

        public void Show(string message)
        {
            Message = message;
            IsBusy = true;
        }

        public void Processing()
        {
            Show("Processing...");
        }

        public void Loading()
        {
            Show("Loading...");
        }

        public void Hide()
        {
            IsBusy = false;
        }
    }
}
