using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using XDeploy.Utils;

namespace XDeploy.IO.InMemory
{
    public class InMemoryDirectory : IDirectory
    {
        // This collection might include not existing files & directories.
        // When a call to GetFile() or GetDirectory() is made, 
        // the file/directory will always be added to this collection not matter if it exists.
        private List<IFileSystemInfo> _fileSystemInfos;
        
        public string Name
        {
            get
            {
                return Path.GetFileName(VirtualPath);
            }
        }

        public string Uri { get; private set; }

        public string VirtualPath { get; private set; }

        public string Extension
        {
            get
            {
                return Path.GetExtension(VirtualPath);
            }
        }

        public bool Exists { get; private set; }

        public bool IsRoot
        {
            get
            {
                return VirtualPath == "/";
            }
        }

        public InMemoryDirectory Parent { get; private set; }

        public InMemoryDirectory(string virtualPath, string uri, InMemoryDirectory parent)
        {
            Require.NotNullOrEmpty(virtualPath, "virtualPath");
            Require.NotNullOrEmpty(uri, "uri");

            VirtualPath = virtualPath;
            Uri = uri;
            Parent = parent;
            _fileSystemInfos = new List<IFileSystemInfo>();
        }

        public void Create()
        {
            Exists = true;
        }

        public InMemoryDirectory CreateSubDirectory(string name)
        {
            var directory = new InMemoryDirectory(VirtualPathUtil.Combine(VirtualPath, name), Path.Combine(Uri, name), this);
            directory.Create();
            _fileSystemInfos.Add(directory);
            return directory;
        }

        public InMemoryFile CreateFile(string fileName, string contents, Encoding encoding)
        {
            Create();
            var stream = new MemoryStream();
            var bytes = encoding.GetBytes(contents);
            stream.Write(bytes, 0, bytes.Length);
            stream.Position = 0;

            return CreateFile(fileName, encoding.GetBytes(contents));
        }

        public InMemoryFile CreateFile(string fileName, byte[] data)
        {
            Create();
            var file = new InMemoryFile(fileName, this, data);
            _fileSystemInfos.Add(file);
            return file;
        }

        public IFile GetFile(string relativeVirtualPath)
        {
            var segments = relativeVirtualPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 1)
            {
                var file = _fileSystemInfos.FirstOrDefault(x => x is IFile && x.Name.Equals(segments[0])) as IFile;
                if (file == null)
                {
                    file = new InMemoryFile(segments[0], this);
                    _fileSystemInfos.Add(file);
                }

                return file;
            }
            else
            {
                var directory = (InMemoryDirectory)GetDirectory(segments[0]);
                return directory.GetFile(String.Join("/", segments.Skip(1)));
            }
        }

        public IDirectory GetDirectory(string relativeVirtualPath)
        {
            var segments = relativeVirtualPath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 1)
            {
                var directory = _fileSystemInfos.FirstOrDefault(x => x is IDirectory && x.Name.Equals(segments[0])) as IDirectory;
                if (directory == null)
                {
                    directory = new InMemoryDirectory(VirtualPathUtil.Combine(VirtualPath, segments[0]), Path.Combine(Uri, segments[0]), this);
                    _fileSystemInfos.Add(directory);
                }

                return directory;
            }
            else
            {
                var directory = (InMemoryDirectory)GetDirectory(segments[0]);
                return directory.GetDirectory(String.Join("/", segments.Skip(1)));
            }
        }

        public IEnumerable<IFile> GetFiles()
        {
            return GetFileSystemInfos().OfType<IFile>();
        }

        public IEnumerable<IDirectory> GetDirectories()
        {
            return GetFileSystemInfos().OfType<IDirectory>();
        }

        public IEnumerable<IFileSystemInfo> GetFileSystemInfos()
        {
            return _fileSystemInfos.Where(x => x.Exists);
        }

        public void Refresh()
        {
        }

        public void Dispose()
        {
        }

        public override string ToString()
        {
            return Uri;
        }
    }
}
