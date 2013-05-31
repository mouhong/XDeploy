using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using System.Windows;
using XDeploy.Workspace.Shell;
using XDeploy.Workspace.Shared;

namespace XDeploy.Workspace.Releases.Screens
{
    [Export(typeof(ReleaseListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReleaseListViewModel : Screen, IPagerHost, ITabContentScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;
        private Func<CreateReleaseViewModel> _createReleaseViewModel;
        private Func<ReleaseDetailViewModel> _releaseDetailViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return Workspace.GetShell();
            }
        }

        public ITabWorkspace Workspace
        {
            get
            {
                return this.GetWorkspace();
            }
        }

        public PagerViewModel Pager { get; private set; }

        public int PageIndex
        {
            get
            {
                return Pager.PageIndex;
            }
        }

        private bool _isLoaded;

        public bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
            set
            {
                if (_isLoaded != value)
                {
                    _isLoaded = value;
                    NotifyOfPropertyChange(() => IsLoaded);
                }
            }
        }

        private bool _hasReleases;

        public bool HasReleases
        {
            get
            {
                return _hasReleases;
            }
            set
            {
                if (_hasReleases != value)
                {
                    _hasReleases = value;
                    NotifyOfPropertyChange(() => HasReleases);
                }
            }
        }

        public BindableCollection<ReleaseListItemViewModel> ReleasesInThisPage { get; private set; }

        [ImportingConstructor]
        public ReleaseListViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<CreateReleaseViewModel> createReleaseViewModelFactory,
            Func<ReleaseDetailViewModel> releaseDetailViewModelFactory)
        {
            _workContextAccessor = workContextAccessor;
            _createReleaseViewModel = createReleaseViewModelFactory;
            _releaseDetailViewModel = releaseDetailViewModelFactory;
            ReleasesInThisPage = new BindableCollection<ReleaseListItemViewModel>();
            Pager = new PagerViewModel(5);
            Pager.PageIndexChanged += Pager_PageIndexChanged;
        }

        private void Pager_PageIndexChanged(object sender, PageIndexChangeEventArgs e)
        {
            LoadAsync(e.NewPageIndex);
        }

        protected override void OnActivate()
        {
            LoadAsync(0, (sender, args) =>
            {
                HasReleases = ReleasesInThisPage.Count > 0;
                IsLoaded = true;
            });
            base.OnActivate();
        }

        public void CreateNewRelease()
        {
            Workspace.ActivateItem(_createReleaseViewModel());
        }

        public void LoadAsync(int pageIndex, EventHandler<ResultCompletionEventArgs> callback = null)
        {
            Coroutine.BeginExecute(LoadPage(pageIndex).GetEnumerator(), null, callback);
        }

        public IEnumerable<IResult> LoadPage(int pageIndex)
        {
            Shell.Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                using (var session = workContext.OpenSession())
                {
                    var releases = session.Query<Release>()
                                          .OrderByDescending(x => x.CreatedAtUtc)
                                          .Paging(Pager.PageSize);

                    ReleasesInThisPage = new BindableCollection<ReleaseListItemViewModel>(
                        releases.Page(pageIndex).ToList().Select(x => new ReleaseListItemViewModel(x)));
                    Pager.Bind(releases, pageIndex);

                    NotifyOfPropertyChange(() => ReleasesInThisPage);
                }
            });

            Shell.Busy.Hide();
        }

        public void ShowDetail(ReleaseListItemViewModel item)
        {
            var detailViewModel = _releaseDetailViewModel();
            detailViewModel.ReleaseId = item.Id;
            Workspace.ActivateItem(detailViewModel);
        }
    }
}
