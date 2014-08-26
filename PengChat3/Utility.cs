using System;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using PC3API_dn;

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

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;

        public static void DisableMaximize(Window win)
        {
            var hwnd = new WindowInteropHelper(win).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }
    }

    public class RoomListItem
    {
        public Room room;

        public RoomListItem(Room r)
        {
            room = r;
        }

        public override string ToString()
        {
            return room.Name;
        }
    }

    public class CntComboBoxItem
    {
        public string Text { get; set; }

        public PengChat3ClientSock Sock { get; set; }

        public void ShutdownSocket()
        {
            if (Sock != null)
            {
                Sock.Logout();
                Sock = null;
            }
        }

        public override string ToString()
        {
            return Sock.ConnectedIP + ":" + Sock.ConnectedPort.ToString() + "   \"" + Sock.Nickname + "\"";
        }
    }
}
