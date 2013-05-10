using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using XDeploy.IO;

namespace XDeploy
{
    public static class ChecksumComputer
    {
        public static string Compute(IFile file)
        {
            Require.NotNull(file, "file");

            using (var stream = file.OpenRead())
            {
                return Compute(stream);
            }
        }

        public static string Compute(FileInfo file)
        {
            Require.NotNull(file, "file");

            using (var stream = file.OpenRead())
            {
                return Compute(stream);
            }
        }

        public static string Compute(string input, Encoding encoding)
        {
            using (var stream = new MemoryStream())
            {
                var bytes = encoding.GetBytes(input);
                stream.Write(bytes, 0, bytes.Length);
                stream.Position = 0;

                return Compute(stream);
            }
        }

        public static string Compute(Stream stream)
        {
            Require.NotNull(stream, "stream");

            var checksum = new StringBuilder();

            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(stream);

                for (var i = 0; i < hash.Length; i++)
                {
                    checksum.Append(hash[i].ToString("x2"));
                }

                return checksum.ToString();
            }
        }
    }
}
