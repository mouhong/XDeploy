using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XDeploy.Workspace.Shared
{
    public class ProgressViewModel : PropertyChangedBase
    {
        private double _minValue;

        public double MinValue
        {
            get
            {
                return _minValue;
            }
            set
            {
                if (_minValue != value)
                {
                    _minValue = value;
                    NotifyOfPropertyChange(() => MinValue);
                }
            }
        }

        private double _maxValue;

        public double MaxValue
        {
            get
            {
                return _maxValue;
            }
            set
            {
                if (_maxValue != value)
                {
                    _maxValue = value;
                    NotifyOfPropertyChange(() => MaxValue);
                }
            }
        }

        private double _value;

        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    NotifyOfPropertyChange(() => Value);
                }
            }
        }

        private bool _isVisible;

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    NotifyOfPropertyChange(() => IsVisible);
                }
            }
        }

        private bool _hasErrors;

        public bool HasErrors
        {
            get
            {
                return _hasErrors;
            }
            set
            {
                if (_hasErrors != value)
                {
                    _hasErrors = value;
                    NotifyOfPropertyChange(() => HasErrors);
                }
            }
        }
    }
}
