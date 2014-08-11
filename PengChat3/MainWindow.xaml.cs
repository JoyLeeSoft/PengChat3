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
            InitImages();

#region Control settings
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
            label_Num.Content = ResourceManager.GetStringByKey("Str_NumOfMember") + " : ";
            label_IsNeedPW.Content = ResourceManager.GetStringByKey("Str_IsNeedPW") + " : ";
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
                    sock.Connect(textBox_IP.Text, App.Port, textBox_ID.Text, passwordBox_PW.Password);
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

                CntComboBoxItem item = new CntComboBoxItem();
                item.Text = sock.ConnectedIP + ":" + sock.ConnectedPort.ToString();
                item.Sock = sock;

                comboBox_ConnectionInfo.Items.Add(item);
                comboBox_ConnectionInfo.SelectedItem = item;

                ChangeStatusConnectionInfoControls(System.Windows.Visibility.Visible);
            }
            else
            {
                Utility.Error(ResourceManager.GetStringByKey("Str_EmptyLabel"));
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            ((CntComboBoxItem)comboBox_ConnectionInfo.SelectedItem).Sock.Dispose();

            comboBox_ConnectionInfo.Items.Remove(comboBox_ConnectionInfo.SelectedItem);

            if (comboBox_ConnectionInfo.Items.IsEmpty == false)
                comboBox_ConnectionInfo.SelectedIndex = 0;
            else
                ChangeStatusConnectionInfoControls(System.Windows.Visibility.Hidden);
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
    }
}
