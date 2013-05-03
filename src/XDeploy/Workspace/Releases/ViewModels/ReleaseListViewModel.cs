using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shared.ViewModels;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class ReleaseListViewModel : Screen, IPagerHost
    {
        public ProjectReleasesViewModel Host { get; private set; }

        public ShellViewModel Shell
        {
            get
            {
                return Host.Shell;
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

        public ReleaseListViewModel(ProjectReleasesViewModel host)
        {
            Host = host;
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
            Host.CreateRelease();
        }

        public IEnumerable<IResult> LoadPage(int pageIndex)
        {
            return Host.LoadReleases(pageIndex);
        }

        public IEnumerable<IResult> ShowDetail(ReleaseListItemViewModel item)
        {
            return Host.ShowDetail(item.Id);
        }
    }
}
