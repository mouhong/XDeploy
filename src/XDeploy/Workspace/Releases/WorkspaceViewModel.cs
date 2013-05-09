using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using NHibernate.Linq;
using System.ComponentModel.Composition;
using XDeploy.Workspace.Releases.Screens;

namespace XDeploy.Workspace.Releases
{
    [Export(typeof(ITabWorkspace))]
    public class WorkspaceViewModel : Conductor<IScreen>.Collection.OneActive, ITabWorkspace
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

        public int DisplayOrder
        {
            get
            {
                return 1;
            }
        }

        [ImportingConstructor]
        public WorkspaceViewModel(
            Func<ReleaseListViewModel> releaseListViewModelFactory)
        {
            DisplayName = "Releases";
            _releaseListViewModel = releaseListViewModelFactory;
        }

        public void ShowReleaseList()
        {
            var listViewModel = _releaseListViewModel();
            ActivateItem(listViewModel);
        }

        protected override void OnViewLoaded(object view)
        {
            ShowReleaseList();
            base.OnViewLoaded(view);
        }
    }
}
