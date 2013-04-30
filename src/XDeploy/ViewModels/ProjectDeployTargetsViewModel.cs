using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Linq;

namespace XDeploy.ViewModels
{
    public class ProjectDeployTargetsViewModel : Conductor<IScreen>
    {
        public DeploymentProjectViewModel Project { get; private set; }

        public WorkContext WorkContext { get; private set; }

        public ProjectDeployTargetsViewModel(DeploymentProjectViewModel project, WorkContext workContext)
        {
            DisplayName = "Deploy Targets";
            Project = project;
            WorkContext = workContext;
        }

        public void CreateNewDeployTarget()
        {
            ChangeActiveItem(new CreateDeployTargetViewModel(), true);
        }

        protected override void OnInitialize()
        {
            Task.Factory.StartNew(() =>
            {
                using (var session = WorkContext.Database.OpenSession())
                {
                    var targets = session.Query<DeployTarget>().OrderBy(x => x.Name).ToList();

                    Execute.OnUIThread(() =>
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
            });

            base.OnInitialize();
        }
    }
}
