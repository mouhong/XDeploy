using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace XDeploy.Models
{
    public class Workspace : ViewModelBase
    {
        private ObservableCollection<PageViewModelBase> _pages;

        public ObservableCollection<PageViewModelBase> Pages
        {
            get
            {
                if (_pages == null)
                {
                    _pages = new ObservableCollection<PageViewModelBase>();
                }
                return _pages;
            }
        }

        private DeploymentProjectViewModel _currentProject;

        public DeploymentProjectViewModel CurrentProject
        {
            get
            {
                return _currentProject;
            }
            set
            {
                _currentProject = value;
                HasProjectLoaded = _currentProject != null;

                foreach (var page in Pages)
                {
                    page.OnProjectLoaded(_currentProject);
                }
            }
        }

        private bool _hasProjectLoaded;

        public bool HasProjectLoaded
        {
            get
            {
                return _hasProjectLoaded;
            }
            set
            {
                if (value != _hasProjectLoaded)
                {
                    _hasProjectLoaded = value;
                    OnPropertyChanged("HasProjectLoaded");
                }
            }
        }

        public bool NotProjectLoaded
        {
            get
            {
                return !HasProjectLoaded;
            }
        }

        public static Workspace Instance = new Workspace();

        public Workspace()
        {
            Pages.Add(new HomePageViewModel(this) { IsSelected = true });
            Pages.Add(new ReleasePackagesViewModel(this));
            Pages.Add(new DeployTargetsViewModel(this));
        }
    }
}
