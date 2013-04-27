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
        static void Main(string[] args)
        {
            var sourceDirectory = @"D:\XDeploy\source";
            var deployLocation = StorageLocation.Create(@"D:\XDeploy\live");

            var settings = new DeploymentSettings()
                               .IgnorePaths("obj", ".cs")
                               .IgnoreItemsWhichAreNotModifiedSinceUtc(DateTime.UtcNow.AddSeconds(-20));

            var deployer = new FileDeployer(sourceDirectory, settings)
            {
                BackupLocation = StorageLocation.Create(@"D:\XDeploy\backup\2013-04-27")
            };

            deployer.Deploy(deployLocation);

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
