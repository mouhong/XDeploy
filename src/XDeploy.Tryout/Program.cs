using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using XDeploy.Storage;

namespace XDeploy.Tryout
{
    class Program
    {
        static readonly string ProjectFilePath = @"D:\XDeploy\Projects\WebFormApp\WebFormApp.xdproj";

        static void Main(string[] args)
        {
            DeployRelease("Patch 1");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeployRelease(string name)
        {
            var project = DeploymentProject.LoadFrom(ProjectFilePath);
            var deployDirectory = Directories.GetDirectory(@"D:\XDeploy\Live");

            project.DeployReleasePackage(name, deployDirectory);
        }

        static void CreateRelease(string name, string releaseNote)
        {
            var project = DeploymentProject.LoadFrom(ProjectFilePath);
            project.CreateReleasePackage(name, releaseNote);
        }

        static void CreateProject()
        {
            var project = new DeploymentProject
            {
                Name = "WebFormApp",
                SourceDirectory = @"E:\Projects\Samples\WebFormApp"
            };

            project.IgnorantRules = new AspNetApplicationIgnorantRulesTemplate().GetDefaultIgnorantRules().ToList();

            project.Save(ProjectFilePath);
        }
    }
}
