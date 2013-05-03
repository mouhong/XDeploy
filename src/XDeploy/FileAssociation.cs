using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace XDeploy
{
    public class FileAssociation
    {
        static RegistryKey Root
        {
            get
            {
                return Registry.CurrentUser;
            }
        }

        // Associate file extension with progID, description, icon and application
        public static void Associate(string extension,
               string progID, string description, string application)
        {
            Require.NotNullOrEmpty(extension, "extension");
            Require.NotNullOrEmpty(progID, "progID");
            Require.NotNullOrEmpty(application, "application");
            Require.NotNullOrEmpty(description, "description");

            Root.CreateSubKey(extension).SetValue("", progID);

            using (var key = Root.CreateSubKey(progID))
            {
                key.SetValue("", description);

                key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(application).Quote() + ",0");
                key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(application).Quote() + " \"%1\"");

                // Tell explorer the file association has been changed
                SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
            }
        }

        // Return true if extension already associated in registry
        public static bool IsAssociated(string extension)
        {
            return (Root.OpenSubKey(extension, false) != null);
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath,
            [Out] StringBuilder lpszShortPath, uint cchBuffer);

        // Return short path format of a file name
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}
