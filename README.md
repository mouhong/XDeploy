#XDeploy#

###Deploy files to different locations with automatic backup###

```csharp

	// Deploy files from local file system to remote FTP server
	var sourceDirectory = @"D:\XDeploy\source";
	var deployLocation = StorageLocation.Create(@"ftp://xxx/live", "userName", "password");

	// Backup live files to local file system
	var backupLocation = StorageLocation.Create(@"D:\XDeploy\backup\2013-04-27");

	// You can also backup live files to a remote FTP server
	// var backupLocation = StorageLocation.Create(@"D:ftp://xxx/backup", "userName", "password");
	
	// Ignore some files
	var settings = new DeploymentSettings()
	                   .IgnorePaths("obj", ".cs")
	                   .IgnoreItemsWhichAreNotModifiedSinceUtc(DateTime.UtcNow.AddSeconds(-20));
	
	// Instantiate a FileDeployer instance and set backup location
	var deployer = new FileDeployer(sourceDirectory, settings)
	{
	    BackupLocation = backupLocation
	};
	
	// Invoke Deploy method to backup live files and deploy new files.
	// Files are backupped only when they are going to be overwritten.
	deployer.Deploy(deployLocation);

```

TODO
-----

- [ ] Windows Desktop UI
- [ ] Deployment Profiles
- [ ] Asynchronous Deployment
- [ ] Unit Tests

