using System;
using System.Windows;

namespace PengChat3
{
    internal static class Utility
    {
        public static void Error(string msg, bool shutdown = false)
        {
            MessageBox.Show(msg, "PengChat3 - error", MessageBoxButton.OK, MessageBoxImage.Error);
            if (shutdown)
                Application.Current.Shutdown(-1);
        }
    }
}
