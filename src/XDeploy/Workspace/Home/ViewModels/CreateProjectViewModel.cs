using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Data;
using XDeploy.Validation;
using XDeploy.Workspace.Shell.ViewModels;

namespace XDeploy.Workspace.Home.ViewModels
{
    public class CreateProjectViewModel : ValidatableScreen
    {
        public ShellViewModel Shell { get; private set; }

        private string _projectName;

        [Required]
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (_projectName != value)
                {
                    _projectName = value;
                    NotifyOfPropertyChange(() => ProjectName);
                    NotifyOfPropertyChange(() => CanCreate);
                }
            }
        }

        private string _sourceDirectory;

        [Required]
        public string SourceDirectory
        {
            get
            {
                return _sourceDirectory;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (_sourceDirectory != value)
                {
                    _sourceDirectory = value;
                    NotifyOfPropertyChange(() => SourceDirectory);
                    NotifyOfPropertyChange(() => CanCreate);
                }
            }
        }

        private string _savingDirectory;

        [Required]
        public string SavingDirectory
        {
            get
            {
                return _savingDirectory;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (_savingDirectory != value)
                {
                    _savingDirectory = value;
                    NotifyOfPropertyChange(() => SavingDirectory);
                    NotifyOfPropertyChange(() => CanCreate);
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
                if (value != null)
                {
                    value = value.Trim();
                }

                if (_ignoredPaths != value)
                {
                    _ignoredPaths = value;
                    NotifyOfPropertyChange(() => IgnoredPaths);
                }
            }
        }

        public bool CanCreate
        {
            get
            {
                return !HasErrors;
            }
        }

        public CreateProjectViewModel(ShellViewModel shell)
        {
            Shell = shell;
        }

        public IEnumerable<IResult> Create()
        {
            var projectFilePath = Path.Combine(SavingDirectory, ProjectName.Trim() + ".xdproj");

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

                Database.InitializeDatabase(Paths.DbFile(SavingDirectory));
            });

            foreach (var result in Shell.OpenProject(projectFilePath))
            {
                yield return result;
            }
        }

        public void Cancel()
        {
            Shell.SwithchToWelcomeScreen();
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

        public void BrowseSavingDirectory()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {               
                var defaultPath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "Projects");

                if (!String.IsNullOrWhiteSpace(ProjectName))
                {
                    defaultPath = Path.Combine(defaultPath, ProjectName.Trim());
                }

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
                        SavingDirectory = path;
                        break;
                    }
                    else
                    {
                        Shell.MessageBox.Error("Please select an empty folder", null);
                    }
                };
            }
        }
    }
}
