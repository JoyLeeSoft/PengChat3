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

using MahApps.Metro.Controls.Dialogs;

using PC3API_dn;

namespace PengChat3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeSettings();

            App.Instance = this;
        }

        private void menuItem_TabClose_Click(object sender, RoutedEventArgs e)
        {
            if (tabControl_Page.SelectedItem != tabItem_Main)
            {
                tabControl_Page.Items.Remove(tabControl_Page.SelectedItem);
            }
            else
            {
                Utility.Error(ResourceManager.GetStringByKey("Str_CannotCloseMainTab"));
            }
        }

        private void comboBox_CntList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel model = GetSelectedViewModel();

            if (model != null)
                listView_RoomList.ItemsSource = model.Rooms;
            else
                listView_RoomList.ItemsSource = null;
        }

        private void button_CreateRoom_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomWindow win = new CreateRoomWindow();
            win.ShowDialog();
        }

        private void button_Logout_Click(object sender, RoutedEventArgs e)
        {
            PengChat3ClientSock sock = GetSelectedSock();

            if (sock != null)
            {
                sock.Logout();
            }
        }

        private void window_Main_Closed(object sender, EventArgs e)
        {
            List<PengChat3ClientSock> tempSockets = new List<PengChat3ClientSock>();
            foreach (var model in viewModel)
            {
                tempSockets.Add(model.Sock);
            }

            foreach (PengChat3ClientSock sock in tempSockets)
            {
                sock.Logout();
            }

            viewModel.Clear();
        }
    }
}
