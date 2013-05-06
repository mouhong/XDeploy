using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XDeploy.Workspace;
using XDeploy.Workspace.DeploymentTargets.Screens;
using XDeploy.Workspace.Home.Screens;
using XDeploy.Workspace.Releases.Screens;
using XDeploy.Workspace.Shell;

namespace XDeploy
{
    public class AppBootstrapper : Bootstrapper<ShellViewModel>
    {
        static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();

        private CompositionContainer container;

        protected override void Configure()
        {
            container = new CompositionContainer(
                new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>()));

            CompositionBatch batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new AppWindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());

            // Home Tab
            batch.AddExportedValue<Func<WelcomeScreenViewModel>>(() => container.GetExportedValue<WelcomeScreenViewModel>());
            batch.AddExportedValue<Func<CreateProjectViewModel>>(() => container.GetExportedValue<CreateProjectViewModel>());
            batch.AddExportedValue<Func<ProjectSummaryViewModel>>(() => container.GetExportedValue<ProjectSummaryViewModel>());

            // Releases Tab
            batch.AddExportedValue<Func<NoReleaseViewModel>>(() => container.GetExportedValue<NoReleaseViewModel>());
            batch.AddExportedValue<Func<CreateReleaseViewModel>>(() => container.GetExportedValue<CreateReleaseViewModel>());
            batch.AddExportedValue<Func<ReleaseListViewModel>>(() => container.GetExportedValue<ReleaseListViewModel>());
            batch.AddExportedValue<Func<ReleaseDetailViewModel>>(() => container.GetExportedValue<ReleaseDetailViewModel>());
            batch.AddExportedValue<Func<DeployToTargetViewModel>>(() => container.GetExportedValue<DeployToTargetViewModel>());

            // Deployment Targets Tab
            batch.AddExportedValue<Func<NoDeploymentTargetViewModel>>(() => container.GetExportedValue<NoDeploymentTargetViewModel>());
            batch.AddExportedValue<Func<CreateDeploymentTargetViewModel>>(() => container.GetExportedValue<CreateDeploymentTargetViewModel>());
            batch.AddExportedValue<Func<EditDeploymentTargetViewModel>>(() => container.GetExportedValue<EditDeploymentTargetViewModel>());
            batch.AddExportedValue<Func<DeploymentTargetListViewModel>>(() => container.GetExportedValue<DeploymentTargetListViewModel>());

            batch.AddExportedValue(container);

            container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = container.GetExportedValues<object>(contract);

            if (exports.Count() > 0)
            {
                return exports.First();
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                IoC.Get<StartupArguments>().SetPath(e.Args[0].Trim());
            }

            Task.Factory.StartNew(() => Safely.Execute(EnsureFileAssociation, "Failed ensuring file association."));

            base.OnStartup(sender, e);
        }

        private void EnsureFileAssociation()
        {
            var extension = ".xd";
            var scope = FileAssociationScope.CurrentUser;
            var exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XDeploy.exe");

            if (!FileAssociation.IsAssociated(extension, scope))
            {
                FileAssociation.Associate(
                    extension, "XDeploy", "XDeploy Project", exePath, exePath + ",1", scope);
            }
        }

        protected override void OnUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
#if !DEBUG
            var messageBox =container.GetExportedValue<IMessageBox>();
            messageBox.Error(e.Exception.Message + Environment.NewLine + e.Exception.StackTrace, null);
            e.Handled = true;
#endif
        }
    }
}
