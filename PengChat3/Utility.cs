using System;
using System.Windows;
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
                Sock.Dispose();
                Sock = null;
            }
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
