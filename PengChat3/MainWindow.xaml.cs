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
                ((ChatTab)tabControl_Page.SelectedItem).CloseChat();
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

            if (win.DialogResult.Value == true)
            {
                GetSelectedSock().CreateRoom(win.RoomName, win.MaxConnectorNum, win.Password);
            }
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

        private void button_Entry_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            var rooms = listView_RoomList.Items.Cast<Room>().ToList();
            var room = rooms.Find(r =>
            {
                return r.ID ==
                    Convert.ToUInt32(b.Tag);
            });
            var generator = listView_RoomList.ItemContainerGenerator;

            ListViewItem container = (ListViewItem)generator.ContainerFromItem(room);

            PasswordBox pwd = (PasswordBox)VisualTreeHelperExtensions.FindVisualChild<PasswordBox>(container);
            
            if (room.IsNeedPassword)
            {
                if (pwd.Password == "")
                {
                    Utility.Error(ResourceManager.GetStringByKey("Str_NeedPassword"));
                    return;
                }

                GetSelectedSock().EntryToRoom(room.ID, pwd.Password);
            }
            else
            {
                GetSelectedSock().EntryToRoom(room.ID);
            }
        }

        private void button_Delete_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            GetSelectedSock().DeleteRoom(Convert.ToUInt32(b.Tag));
        }
    }
}