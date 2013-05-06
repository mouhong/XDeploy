using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace XDeploy
{
    public enum FileAssociationScope
    {
        CurrentUser = 0,
        LocalMachine = 1,
        ClassRoot = 2
    }

    public static class FileAssociation
    {
        public static void Associate(string extension,
               string progID, string description, string application, FileAssociationScope scope)
        {
            Associate(extension, progID, description, application, application + ",0", scope);
        }

        public static void Associate(string extension,
               string progID, string description, string application, string icon, FileAssociationScope scope)
        {
            Require.NotNullOrEmpty(extension, "extension");
            Require.NotNullOrEmpty(progID, "progID");
            Require.NotNullOrEmpty(application, "application");
            Require.NotNullOrEmpty(description, "description");

            var basePath = GetRegistryKeyBasePath(scope);

            Registry.SetValue(basePath + extension, "", progID);
            Registry.SetValue(basePath + progID + "\\DefaultIcon", "", icon);
            Registry.SetValue(basePath + progID + "\\Shell\\Open\\Command", "", application.Quote() + " " + "%1".Quote());

            SHChangeNotify(0x08000000, 0x0000, IntPtr.Zero, IntPtr.Zero);
        }

        static string GetRegistryKeyBasePath(FileAssociationScope scope)
        {
            if (scope == FileAssociationScope.ClassRoot)
            {
                return "HKEY_CLASSES_ROOT\\";
            }
            if (scope == FileAssociationScope.LocalMachine)
            {
                return "HKEY_LOCAL_MACHINE\\Software\\Classes\\";
            }

            return "HKEY_CURRENT_USER\\Software\\Classes\\";
        }

        public static bool IsAssociated(string extension, FileAssociationScope scope)
        {
            var keyPath = GetRegistryKeyBasePath(scope) + extension;
            return Registry.GetValue(keyPath, "", "") != null;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    }
}
