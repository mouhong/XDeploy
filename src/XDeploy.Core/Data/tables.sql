create table HiValue
(
	TableName varchar(50) not null,
	NextValue int not null,

	constraint PK_HiValue primary key (TableName)
);

create table DeploymentTarget
(
	Id int not null,
	Name nvarchar(50) not null,
	DeployLocation_Uri nvarchar(255) not null,
	DeployLocation_UserName nvarchar(50) null,
	DeployLocation_Password nvarchar(50) null,
	BackupRootLocation_Uri nvarchar(255) null,
	BackupRootLocation_UserName nvarchar(50) null,
	BackupRootLocation_Password nvarchar(50) null,
	BackupFolderNameTemplate nvarchar(100) null,
	CreatedAtUtc datetime not null,
	LastDeployedAtUtc datetime null,
	LastBackuppedAtUtc datetime null,

	constraint PK_DeploymentTarget primary key (Id)
);

create table Release
(
	Id int not null,
	Name nvarchar(50) not null,
	ReleaseNotes text null,
	CreatedAtUtc datetime not null,
	LastDeployedAtUtc datetime null,
	DeploymentInfos text null,

	constraint PK_Release primary key (Id)
);

create table DeploymentLog
(
	Id int not null,
	ReleaseId int not null,
	DeployTargetId int not null,
	DeployTargetName nvarchar(50) not null,
	Success bit not null,
	[Message] text null,
	StartedAtUtc datetime not null,
	CompletedAtUtc datetime not null,
	DeploymentStartedAtUtc datetime not null,
	DeploymentCompletedAtUtc datetime not null,
	BackupStartedAtUtc datetime null,
	BackupCompletedAtUtc datetime null,

	constraint PK_DeployLog primary key (Id),
	constraint FK_DeployLog_ReleaseId foreign key (ReleaseId) references Release(Id) on delete cascade
);

create table FileChecksum
(
	VirtualPath nvarchar(300) not null,
	[Checksum] varchar(32) not null,
	LastUpdatedAtUtc datetime not null,

	constraint PK_FileState primary key (VirtualPath)
);

insert into HiValue values ('DeploymentTarget', 0);
insert into HiValue values ('Release', 0);
insert into HiValue values ('DeploymentLog', 0);
