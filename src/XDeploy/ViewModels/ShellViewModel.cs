using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using XDeploy.Events;

namespace XDeploy.ViewModels
{
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : PropertyChangedBase, IHandle<CurrentProjectChanged>
    {
        private IEventAggregator _events;
        private PropertyChangedBase _workspace;

        public PropertyChangedBase Workspace
        {
            get
            {
                return _workspace;
            }
            set
            {
                if (_workspace != null)
                {
                    _workspace = value;
                    NotifyOfPropertyChange(() => Workspace);
                }
            }
        }

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            _workspace = new WelcomeScreenViewModel(events);
        }

        public void Handle(CurrentProjectChanged message)
        {
            Workspace = new ProjectWorkspaceViewModel(message.NewProject);
        }
    }
}
