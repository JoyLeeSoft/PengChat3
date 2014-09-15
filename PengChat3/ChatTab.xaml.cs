using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using PC3API_dn;

namespace PengChat3
{
    /// <summary>
    /// Interaction logic for ChatTab.xaml
    /// </summary>
    public partial class ChatTab : MahApps.Metro.Controls.MetroTabItem
    {
        private PengChat3ClientSock Sock;
        private Room Room;

        public ChatTab(PengChat3ClientSock sock, Room room)
        {
            InitializeComponent();

            Sock = sock;
            Room = room;

            Header = Room.Name;
        }
    }
}
