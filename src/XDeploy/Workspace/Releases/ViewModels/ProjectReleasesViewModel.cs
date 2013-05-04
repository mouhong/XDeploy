using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using XDeploy.Workspace.Shell.ViewModels;
using NHibernate.Linq;

namespace XDeploy.Workspace.Releases.ViewModels
{
    public class ProjectReleasesViewModel : Conductor<IScreen>.Collection.OneActive, IReleaseCreationFormHost
    {
        public ShellViewModel Shell { get; private set; }

        public NoReleaseViewModel NoReleaseScreen { get; private set; }

        public CreateReleaseViewModel CreateReleaseScreen { get; private set; }

        public ReleaseListViewModel ReleaseListScreen { get; private set; }

        public ReleaseDetailViewModel DetailScreen { get; private set; }

        public DeploymentViewModel DeploymentScreen { get; private set; }

        public int PageIndex { get; private set; }

        public ProjectReleasesViewModel(ShellViewModel shell)
        {
            DisplayName = "Releases";
            Shell = shell;

            NoReleaseScreen = new NoReleaseViewModel(this);
            CreateReleaseScreen = new CreateReleaseViewModel(this, Shell.WorkContext.Project);
            ReleaseListScreen = new ReleaseListViewModel(this);
            DetailScreen = new ReleaseDetailViewModel(this);
            DeploymentScreen = new DeploymentViewModel(this);
        }

        public void CreateRelease()
        {
            CreateReleaseScreen.Reset();
            ActivateItem(CreateReleaseScreen);
        }

        public void ShowDeploymentScreen(ReleaseDetailViewModel release, TargetDeploymentInfoViewModel target)
        {
            DeploymentScreen.Update(release, target);
            ActivateItem(DeploymentScreen);
        }

        public IEnumerable<IResult> ShowDetail(int releaseId)
        {
            Shell.Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                using (var session = Shell.WorkContext.OpenSession())
                {
                    var release = session.Get<Release>(releaseId);
                    var targets = session.Query<DeploymentTarget>()
                                         .OrderBy(x => x.Id)
                                         .ToList();
                    DetailScreen.UpdateFrom(release, targets);
                }
            });

            Shell.Busy.Hide();

            ActivateItem(DetailScreen);
        }

        protected override void OnViewLoaded(object view)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadReleases", (DependencyObject)view, parameters: new object[] { 0 });
            base.OnViewLoaded(view);
        }

        public IEnumerable<IResult> LoadReleases(int pageIndex)
        {
            PageIndex = pageIndex;

            Shell.Busy.Loading();

            yield return new AsyncActionResult(context =>
            {
                using (var session = Shell.WorkContext.OpenSession())
                {
                    var releases = session.Query<Release>()
                                          .OrderByDescending(x => x.Id)
                                          .Paging(ReleaseListScreen.Pager.PageSize);

                    if (releases.Count > 0)
                    {
                        ReleaseListScreen.Update(releases, pageIndex);
                        ActivateItem(ReleaseListScreen);
                    }
                    else
                    {
                        ActivateItem(NoReleaseScreen);
                    }
                }
            });

            Shell.Busy.Hide();
        }

        public void OnReleaseCreated(CreateReleaseViewModel viewModel)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadReleases", (DependencyObject)GetView(), parameters: new object[] { 0 });
        }

        public void OnReleaseCreationCanceled(CreateReleaseViewModel viewModel)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadReleases", (DependencyObject)GetView(), parameters: new object[] { PageIndex });
        }
    }
}
