using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using XDeploy.Workspace.Shell.ViewModels;
using NHibernate.Linq;
using System.ComponentModel.Composition;

namespace XDeploy.Workspace.Releases.ViewModels
{
    [Export(typeof(IWorkspace))]
    public class ReleasesWorkspaceViewModel : Conductor<IScreen>.Collection.OneActive, IWorkspace
    {
        private Func<ReleaseListViewModel> _releaseListViewModel;

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

        [ImportingConstructor]
        public ReleasesWorkspaceViewModel(
            Func<ReleaseListViewModel> releaseListViewModelFactory)
        {
            DisplayName = "Releases";
            _releaseListViewModel = releaseListViewModelFactory;
        }

        public void ShowReleaseList()
        {
            var listViewModel = _releaseListViewModel();
            ActivateItem(listViewModel);
            listViewModel.LoadAsync(0);
        }

        protected override void OnViewLoaded(object view)
        {
            ShowReleaseList();
            base.OnViewLoaded(view);
        }
    }
}
