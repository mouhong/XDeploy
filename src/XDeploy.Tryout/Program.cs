using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using XDeploy.Project;
using XDeploy.Storage;

namespace XDeploy.Tryout
{
    class Program
    {
        static void Main(string[] args)
        {
            var project = new DeploymentProject
            {
                Name = "Weixin",
                SourceDirectory = @"E:\Projects\Mine\Weixin\src\Weinxin.Web",
                IgnorantRules =
                {
                    new PathIgnorantRule(new [] { ".cs", ".user", ".pdb", ".csproj", "TestResults", "/App_Data/Assemblies", "/App_Data/logs" })
                }
            };

            project.Profiles.Add(new DeploymentProfile("Local Deploy")
            {
                DeployLocation = new Location(@"D:\XDeploy\Weixin\Live"),
                BackupLocation = new Location(@"D:\XDeploy\Weixin\Backup")
            });
            project.Profiles.Add(new DeploymentProfile("Live Deploy")
            {
                DeployLocation = new Location(@"ftp://ftp01.site4future.com/Live", "sym1987-004", "abc123"),
                BackupLocation = new Location(@"D:\XDeploy\Weixin\Backup")
            });

            project.Save(@"D:\Weixin.xdproj");
            

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
