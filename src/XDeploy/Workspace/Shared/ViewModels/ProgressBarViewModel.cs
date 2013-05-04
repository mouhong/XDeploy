using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace XDeploy.Workspace.Shared.ViewModels
{
    public class ProgressBarViewModel : PropertyChangedBase
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

        private Brush _foreground = Brushes.Green;

        public Brush Foreground
        {
            get
            {
                return _foreground;
            }
            set
            {
                if (_foreground != value)
                {
                    _foreground = value;
                    NotifyOfPropertyChange(() => Foreground);
                }
            }
        }

        public void SetError()
        {
            Foreground = Brushes.Red;
        }

        public void ResetColor()
        {
            Foreground = Brushes.Green;
        }
    }
}
