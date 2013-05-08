using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;

namespace XDeploy.Storage.Ftp
{
    public class LazyFtpClient : IDisposable
    {
        private FtpClient _value;

        public FtpClient Value
        {
            get
            {
                if (_value == null)
                {
                    var client = new FtpClient
                    {
                        Credentials = Credential,
                        Host = Host,
                        Port = Port
                    };
                    _value = client;
                }

                return _value;
            }
        }

        public string Host { get; private set; }

        public int Port { get; private set; }

        public NetworkCredential Credential { get; private set; }

        public bool IsConnected
        {
            get
            {
                return Value.IsConnected;
            }
        }

        public LazyFtpClient(string host, NetworkCredential credential, int port = 21)
        {
            Host = host;
            Credential = credential;
            Port = port;
        }

        public void Connect()
        {
            Value.Connect();
        }

        public void CreateDirectory(string path, bool force)
        {
            Value.CreateDirectory(path, force);
        }

        public bool DirectoryExists(string path)
        {
            return Value.DirectoryExists(path);
        }

        public bool FileExists(string path)
        {
            return Value.FileExists(path, FtpListOption.ForceList | FtpListOption.AllFiles);
        }

        public void DeleteFile(string path)
        {
            Value.DeleteFile(path);
        }

        public Stream OpenRead(string path)
        {
            return Value.OpenRead(path);
        }

        public Stream OpenWrite(string path)
        {
            return Value.OpenWrite(path);
        }

        public void Rename(string path, string dest)
        {
            Value.Rename(path, dest);
        }

        public void Dispose()
        {
            if (_value != null)
            {
                _value.Dispose();
            }
        }
    }
}
