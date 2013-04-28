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
            DeployRelease("Release 1.2", "Live Site");

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        static void DeployRelease(string name, string target)
        {
            var project = DeploymentProject.LoadFrom(ProjectFilePath);
            project.DeployReleasePackage(name, target);
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

            project.DeployTargets.Add(new DeployTarget
            {
                Name ="Local Test Site",
                DeployLocation = new Location(@"D:\XDeploy\TestSite")
            });
            project.DeployTargets.Add(new DeployTarget
            {
                Name = "Live Site",
                DeployLocation = new Location(@"ftp://ftp.website.com/LiveSite", "user", "abc123"),
                BackupLocation = new Location(@"D:\XDeploy\Backup")
            });

            project.Save(ProjectFilePath);
        }
    }
}
