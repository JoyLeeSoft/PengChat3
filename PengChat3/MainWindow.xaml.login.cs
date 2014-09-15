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
    public partial class MainWindow
    {
        private void LoginToServer()
        {
            if (textBox_ID.Text != "" && textBox_PW.Password != "" && textBox_IP.Text != "")
            {
                PengChat3ClientSock sock = new PengChat3ClientSock();

                try
                {
                    sock.OnLogin += sock_OnLogin;
                    sock.OnDisconnected += sock_OnDisconnected;
                    sock.OnRoomInfo += sock_OnRoomInfo;
                    sock.OnCreateRoom += sock_OnCreateRoom;
                    sock.OnRemoveRoom += sock_OnRemoveRoom;
                    sock.OnAddClient += sock_OnAddClient;
                    //sock.OnRemoveClient += sock_OnRemoveClient;
                    //sock.OnGetMembers += sock_OnGetMembers;
                    //sock.OnChangeState += sock_OnChangeState;
                    //sock.OnChangeMaster += sock_OnChangeMaster;

                    sock.Connect(textBox_IP.Text, App.Port);
                    sock.Login(textBox_ID.Text, textBox_PW.Password);
                }
                catch (System.Net.Sockets.SocketException ex)
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

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginToServer();
        }

        private void textBox_login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                LoginToServer();
        }
    }
}
