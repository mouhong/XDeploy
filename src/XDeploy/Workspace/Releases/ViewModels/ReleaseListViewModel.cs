using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using NHibernate.Linq;
using XDeploy.Workspace.Shared.ViewModels;
using XDeploy.Workspace.Shell.ViewModels;
using System.Windows;

namespace XDeploy.Workspace.Releases.ViewModels
{
    [Export(typeof(ReleaseListViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReleaseListViewModel : Screen, IPagerHost, IWorkspaceScreen
    {
        private IProjectWorkContextAccessor _workContextAccessor;
        private Func<CreateReleaseViewModel> _createReleaseViewModel;
        private Func<NoReleaseViewModel> _noReleaseViewModel;
        private Func<ReleaseDetailViewModel> _releaseDetailViewModel;

        public ShellViewModel Shell
        {
            get
            {
                return Workspace.GetShell();
            }
        }

        public IWorkspace Workspace
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

        public BindableCollection<ReleaseListItemViewModel> ReleasesInThisPage { get; private set; }

        [ImportingConstructor]
        public ReleaseListViewModel(
            IProjectWorkContextAccessor workContextAccessor,
            Func<CreateReleaseViewModel> createReleaseViewModelFactory,
            Func<NoReleaseViewModel> noReleaseViewModelFactory,
            Func<ReleaseDetailViewModel> releaseDetailViewModelFactory)
        {
            _workContextAccessor = workContextAccessor;
            _createReleaseViewModel = createReleaseViewModelFactory;
            _noReleaseViewModel = noReleaseViewModelFactory;
            _releaseDetailViewModel = releaseDetailViewModelFactory;
            ReleasesInThisPage = new BindableCollection<ReleaseListItemViewModel>();
            Pager = new PagerViewModel(this, 6);
        }

        public void Update(PagedQueryable<Release> query, int pageIndex)
        {
            Pager.Update(query, pageIndex);

            ReleasesInThisPage = new BindableCollection<ReleaseListItemViewModel>(
                query.Page(pageIndex).Select(x => new ReleaseListItemViewModel(x)));

            NotifyOfPropertyChange(() => ReleasesInThisPage);
            NotifyOfPropertyChange(() => Pager);
        }

        public void CreateNewRelease()
        {
            Workspace.ActivateItem(_createReleaseViewModel());
        }

        public void LoadAsync(int pageIndex)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadPage", (DependencyObject)GetView(), parameters: new object[] { pageIndex });
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
                                          .OrderByDescending(x => x.Id)
                                          .Paging(Pager.PageSize);

                    if (releases.Count > 0)
                    {
                        Update(releases, pageIndex);
                        Workspace.ActivateItem(this);
                    }
                    else
                    {
                        Workspace.ActivateItem(_noReleaseViewModel());
                    }
                }
            });

            Shell.Busy.Hide();
        }

        public IEnumerable<IResult> ShowDetail(ReleaseListItemViewModel item)
        {
            Shell.Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                var workContext = _workContextAccessor.GetCurrentWorkContext();

                using (var session = workContext.OpenSession())
                {
                    var release = session.Get<Release>(item.Id);
                    var targets = session.Query<DeploymentTarget>()
                                         .OrderBy(x => x.Id)
                                         .ToList();

                    var detailViewModel = _releaseDetailViewModel();
                    detailViewModel.UpdateFrom(release, targets);
                    Workspace.ActivateItem(detailViewModel);
                }
            });

            Shell.Busy.Hide();
        }
    }
}
