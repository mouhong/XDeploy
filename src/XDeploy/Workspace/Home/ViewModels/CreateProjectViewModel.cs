using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Home.ViewModels
{
    public class CreateProjectViewModel : Screen
    {
        public ShellViewModel Shell { get; private set; }

        private string _projectName;

        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                NotifyOfPropertyChange(() => ProjectName);
            }
        }

        private string _sourceDirectory;

        public string SourceDirectory
        {
            get
            {
                return _sourceDirectory;
            }
            set
            {
                if (_sourceDirectory != value)
                {
                    _sourceDirectory = value;
                    NotifyOfPropertyChange(() => SourceDirectory);
                }
            }
        }

        private string _ignoredPaths;

        public string IgnoredPaths
        {
            get
            {
                return _ignoredPaths;
            }
            set
            {
                if (_ignoredPaths != value)
                {
                    _ignoredPaths = value;
                    NotifyOfPropertyChange(() => IgnoredPaths);
                }
            }
        }

        public CreateProjectViewModel(ShellViewModel shell)
        {
            Shell = shell;
        }

        public IEnumerable<IResult> Create()
        {
            var savingDirectory = AskForProjectSavingDirectory();

            if (String.IsNullOrWhiteSpace(savingDirectory))
            {
                yield break;
            }

            var projectFilePath = Path.Combine(savingDirectory, ProjectName.Trim() + ".xdproj");

            Shell.Busy.Show("Creating project...");

            yield return new AsyncActionResult(context =>
            {
                var project = new DeploymentProject
                {
                    Name = ProjectName.Trim(),
                    SourceDirectory = SourceDirectory.Trim()
                };

                if (!String.IsNullOrWhiteSpace(IgnoredPaths))
                {
                    project.IgnorantRules.Add(new PathIgnorantRule
                    {
                        IgnoredPaths = IgnoredPaths.Trim()
                    });
                }

                project.Save(projectFilePath);
            });

            foreach (var result in Shell.OpenProject(projectFilePath))
            {
                yield return result;
            }
        }

        public void Cancel()
        {
            Shell.ShowWelcomeScreen();
        }

        public void BrowseSourceDirectory()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    SourceDirectory = dialog.SelectedPath;
                }
            }
        }

        public string AskForProjectSavingDirectory()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Choose saving path for this XDeploy project:";
                
                var defaultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Projects", ProjectName.Trim());

                if (!Directory.Exists(defaultPath))
                {
                    Directory.CreateDirectory(defaultPath);
                }

                dialog.SelectedPath = defaultPath;

                while (true)
                {
                    var result = dialog.ShowDialog();

                    if (result != System.Windows.Forms.DialogResult.OK)
                    {
                        break;
                    }

                    var path = dialog.SelectedPath;

                    if (!Directory.EnumerateFileSystemEntries(path).Any())
                    {
                        return path;
                    }
                    else
                    {
                        Shell.MessageBox.Error("Please select an empty folder", null);
                    }
                };
            }

            return null;
        }
    }
}
