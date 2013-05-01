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
    public class ProjectDeploymentTargetsViewModel : Conductor<IScreen>.Collection.OneActive, IDeploymentTargetFormActionAware
    {
        public ShellViewModel Shell { get; private set; }

        public DeploymentProjectViewModel Project { get; private set; }

        public WorkContext WorkContext { get; private set; }

        public CreateDeploymentTargetViewModel CreationScreen { get; private set; }

        public NoDeploymentTargetViewModel NoTargetScreen { get; private set; }

        public DeploymentTargetListViewModel ListScreen { get; private set; }

        public ProjectDeploymentTargetsViewModel(ShellViewModel shell, DeploymentProjectViewModel project, WorkContext workContext)
        {
            DisplayName = "Deployment Targets";
            Shell = shell;
            Project = project;
            WorkContext = workContext;

            CreationScreen = new CreateDeploymentTargetViewModel(shell, this);
            NoTargetScreen = new NoDeploymentTargetViewModel(this);
            ListScreen = new DeploymentTargetListViewModel();

            Items.Add(CreationScreen);
            Items.Add(NoTargetScreen);
            Items.Add(ListScreen);
        }

        public void CreateDeploymentTarget()
        {
            CreationScreen.Reset();
            ActivateItem(CreationScreen);
        }

        protected override void OnViewLoaded(object view)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadTargets", (DependencyObject)view);
        }

        public IEnumerable<IResult> LoadTargets()
        {
            Shell.Busy.Loading();

            List<DeploymentTarget> targets = null;

            yield return new AsyncActionResult(context =>
            {
                using (var session = WorkContext.Database.OpenSession())
                {
                    targets = session.Query<DeploymentTarget>().OrderBy(x => x.Name).ToList();
                }
            });

            Shell.Busy.Hide();

            if (targets == null) yield break;

            yield return new ActionResult(context =>
            {
                if (targets.Count == 0)
                {
                    ActivateItem(NoTargetScreen);
                }
                else
                {
                    ActivateItem(ListScreen);
                }
            });
        }

        public void OnFormCanceled(DeploymentTargetFormViewModel viewModel, FormMode mode)
        {
            if (ListScreen.TotalTargets > 0)
            {
                ActivateItem(ListScreen);
            }
            else
            {
                ActivateItem(NoTargetScreen);
            }
        }

        public void OnFormSaved(DeploymentTargetFormViewModel viewModel, FormMode mode)
        {
            Caliburn.Micro.Action.Invoke(this, "LoadTargets", (DependencyObject)GetView());
        }
    }
}
