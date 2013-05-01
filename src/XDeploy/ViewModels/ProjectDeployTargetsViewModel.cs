using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;
using System.Windows;

namespace XDeploy.ViewModels
{
    public class ProjectDeployTargetsViewModel : Conductor<IScreen>
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentProjectViewModel Project { get; private set; }

        public WorkContext WorkContext { get; private set; }

        public ProjectDeployTargetsViewModel(ShellViewModel shell, DeploymentProjectViewModel project, WorkContext workContext)
        {
            DisplayName = "Deploy Targets";
            Shell = shell;
            Project = project;
            WorkContext = workContext;
        }

        public void CreateNewDeployTarget()
        {
            ChangeActiveItem(new CreateDeployTargetViewModel(), true);
        }

        protected override void OnViewLoaded(object view)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadDeployTargets", (DependencyObject)view);
        }

        public IEnumerable<IResult> LoadDeployTargets()
        {
            Shell.Busy.Show("Loading...");

            List<DeployTarget> targets = null;

            yield return new AsyncActionResult(context =>
            {
                using (var session = WorkContext.Database.OpenSession())
                {
                    targets = session.Query<DeployTarget>().OrderBy(x => x.Name).ToList();
                }
            });

            Shell.Busy.Hide();

            if (targets == null) yield break;

            yield return new ActionResult(context =>
            {
                if (targets.Count == 0)
                {
                    ActivateItem(new NoDeployTargetViewModel(this));
                }
                else
                {
                    ActivateItem(new DeployTargetListViewModel());
                }
            });
        }
    }
}
