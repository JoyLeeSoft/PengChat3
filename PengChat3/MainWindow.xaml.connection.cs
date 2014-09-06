using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class ViewModel
    {
        public PengChat3ClientSock Sock { get; internal set; }

        public ObservableCollection<Room> Rooms { get; internal set; }

        public ViewModel(PengChat3ClientSock sock)
        {
            Sock = sock;
            Rooms = new ObservableCollection<Room>();
        }
    }

    public partial class MainWindow
    {
        public ObservableCollection<ViewModel> viewModel { get; set; }
    }
}
