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
using XDeploy.Workspace.Shell.ViewModels;

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

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
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
            if (!FileAssociation.IsAssociated(".xdproj"))
            {
                FileAssociation.Associate(".xdproj", "XDeploy", "XDeploy Project", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "XDeploy.exe"));
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
