#define TEST

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

#region Control name settings
            textBlock_tabItemMain.Text = ResourceManager.GetStringByKey("Str_MainPage");
            textBlock_groupBoxLogin.Text = ResourceManager.GetStringByKey("Str_Login");
            label_ID.Content = ResourceManager.GetStringByKey("Str_ID") + " : ";
            label_PW.Content = ResourceManager.GetStringByKey("Str_PW") + " : ";
            label_IP.Content = ResourceManager.GetStringByKey("Str_IP") + " : ";
            textBlock_LoginButton.Text = ResourceManager.GetStringByKey("Str_Login");
            textBlock_groupBoxConnection.Text = ResourceManager.GetStringByKey("Str_ConnectionInfo");
            gridViewColumn_RoomName.Header = ResourceManager.GetStringByKey("Str_RoomName");
            textBlock_GroupBoxRoomInfo.Text = ResourceManager.GetStringByKey("Str_RoomInfo");
            label_RoomName.Content = ResourceManager.GetStringByKey("Str_NoSelectedRoom");
            label_Master.Content = ResourceManager.GetStringByKey("Str_Master") + " : ";
            label_Num.Content = ResourceManager.GetStringByKey("Str_MaxConnectorNum") + " : ";
            label_PWRoom.Content = ResourceManager.GetStringByKey("Str_PW") + " : ";
            textBlock_SigninButton.Text = ResourceManager.GetStringByKey("Str_Signin");
            textBlock_DeleteRoomButton.Text = ResourceManager.GetStringByKey("Str_Delete");
            textBlock_LogoutButton.Text = ResourceManager.GetStringByKey("Str_Logout");
            textBlock_CreateRoomButton.Text = ResourceManager.GetStringByKey("Str_CreateRoom");
            textBlock_GroupBoxInfo.Text = ResourceManager.GetStringByKey("Str_InfoWindow");
#endregion

#if TEST
            textBox_ID.Text = "1";
            passwordBox_PW.Password = "1";
            textBox_IP.Text = "127.0.0.1";
#endif
            textBox_ID.Focus();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_ID.Text != "" && passwordBox_PW.Password != "" && textBox_IP.Text != "")
            {
                PengChat3ClientSock sock = new PengChat3ClientSock();

                try
                {
                    sock.OnLogin += sock_OnLogin;
                    sock.OnDisconnected += sock_OnDisconnected;
                    sock.OnRoomInfo += sock_OnRoomInfo;
                    sock.OnCreateRoom += sock_OnCreateRoom;
                    sock.OnDeleteRoom += sock_OnDeleteRoom;
                    sock.OnAddClient += sock_OnAddClient;
                    sock.Connect(textBox_IP.Text, App.Port);
                    sock.Login(textBox_ID.Text, passwordBox_PW.Password);
                }
                catch (Exception ex)
                {
                    Utility.Error(ResourceManager.GetStringByKey("Str_CannotConnectToServer") + '\n' + ex.Message);

                    if (sock != null)
                    {
                        sock.Dispose();
                        sock = null;
                    }

                    return;
                }
            }
            else
            {
                Utility.Error(ResourceManager.GetStringByKey("Str_EmptyLabel"));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedSock().Logout();

            /*comboBox_ConnectionInfo.Items.Remove(comboBox_ConnectionInfo.SelectedItem);

            ChangeStatusRoomInfoControls(Visibility.Hidden, null, null);

            if (comboBox_ConnectionInfo.Items.IsEmpty == false)
                comboBox_ConnectionInfo.SelectedIndex = 0;
            else
                ChangeStatusConnectionInfoControls(Visibility.Hidden);*/
        }

        private void LoginTextboxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginButton_Click(null, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (CntComboBoxItem item in comboBox_ConnectionInfo.Items)
            {
                item.ShutdownSocket();
            }

            comboBox_ConnectionInfo.Items.Clear();
        }

        private void listView_RoomInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView_RoomInfo.Items.IsEmpty == false)
            {
                var r = GetSelectedRoomItem();

                if (r != null)
                    ChangeStatusRoomInfoControls(Visibility.Visible, r,
                        GetSelectedSock().Nickname);
            }
        }

        private void comboBox_ConnectionInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeStatusRoomInfoControls(Visibility.Hidden, null, null);

            listView_RoomInfo.Items.Clear();

            if (comboBox_ConnectionInfo.Items.IsEmpty == false && comboBox_ConnectionInfo.SelectedItem != null)
            {
                foreach (var r in GetSelectedSock().Rooms)
                    listView_RoomInfo.Items.Add(new RoomListItem(r));
            }
        }

        private void CreateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            CreateRoomWindow win = new CreateRoomWindow();
            win.ShowDialog();

            if (win.DialogResult.Value == true)
            {
                GetSelectedSock().CreateRoom(win.RoomName, win.MaxConnectorNum, win.Password);
            }
        }

        private void SigninButton_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox_RoomPW.Password != "")
                GetSelectedSock().EntryToRoom(GetSelectedRoomItem().room.ID, passwordBox_RoomPW.Password);
            else
                GetSelectedSock().EntryToRoom(GetSelectedRoomItem().room.ID);
        }

        private void DeleteRoomButton_Click(object sender, RoutedEventArgs e)
        {
            GetSelectedSock().DeleteRoom(GetSelectedRoomItem().room.ID);
        }
    }
}
