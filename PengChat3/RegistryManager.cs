using System;
using Microsoft.Win32;

namespace PengChat3
{
    internal sealed class RegistryManager
    {
        public const string DefaultPath = @"Software\PengChat3";
        public const string LanguagePackName = "LanguagePackName";
        private static RegistryKey Key;

        public static bool OpenRegistry()
        {
            // Open PengChat3 registry
            try
            {
                Key = Registry.CurrentUser.OpenSubKey(DefaultPath, true);

                // If failed
                if (Key == null)
                    // Try create registry
                    Key = Registry.CurrentUser.CreateSubKey(DefaultPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (Exception)
            {
                return false;
            }

            // If also failed
            if (Key == null)
                return false;

            return true;
        }

        public static bool IsOpened()
        {
            return Key != null;
        }

        public static void CloseRegistry()
        {
            Key.Close();
        }

        public static object ReadValue(string name)
        {
            try
            {
                object o = Key.GetValue(name);
                return o;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void WriteValue(string name, object value)
        {
            try
            {
                Key.SetValue(name, value);
            }
            catch (Exception)
            {

            }
        }
    }
}