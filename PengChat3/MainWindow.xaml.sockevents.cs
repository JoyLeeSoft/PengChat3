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
using System.Windows.Threading;
using PC3API_dn;

namespace PengChat3
{
    public partial class MainWindow
    {
        void sock_OnLogin(object sender, LoginEventArgs e)
        {
            switch (e.ErrCode)
            {
                case LoginEventArgs.ErrorCode.Ok:
                    Logging(ResourceManager.GetStringByKey("Str_SuccessedConnect") + '\n' +
                        string.Format("{0}:{1}", e.ConnectedIP, e.ConnectedPort));

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        CntComboBoxItem item = new CntComboBoxItem();
                        item.Text = e.ConnectedIP + ":" + e.ConnectedPort.ToString();
                        item.Sock = (PengChat3ClientSock)sender;

                        comboBox_ConnectionInfo.Items.Add(item);
                        comboBox_ConnectionInfo.SelectedItem = item;

                        ChangeStatusConnectionInfoControls(System.Windows.Visibility.Visible);
                    }));
                    break;
                case LoginEventArgs.ErrorCode.UnknownIdPw:
                    string err = ResourceManager.GetStringByKey("Str_NotSuccessedConnect");
                    Logging(err);
                    Utility.Error(err);
                    break;
            }
        }

        void sock_OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            PengChat3ClientSock sock = (PengChat3ClientSock)sender;

            switch (e.ErrCode)
            {
                case DisconnectedEventArgs.ErrorCode.ServerError:
                    string err = ResourceManager.GetStringByKey("Str_DisconnectedServer") + '\n' 
                    + e.DisconnectedIP + ':' + e.DisconnectedPort.ToString();
                    Logging(err);
                    Utility.Error(err);
                    break;

                case DisconnectedEventArgs.ErrorCode.Logout:
                    string msg = ResourceManager.GetStringByKey("Str_LoggedOut") + '\n' 
                    + e.DisconnectedIP + ':' + e.DisconnectedPort.ToString();
                    Logging(msg);
                    break;
            }

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (CntComboBoxItem item in comboBox_ConnectionInfo.Items)
                {
                    if (item.Sock == sock)
                    {
                        comboBox_ConnectionInfo.Items.Remove(item);
                        break;
                    }
                }

                if (comboBox_ConnectionInfo.Items.IsEmpty)
                {
                    ChangeStatusConnectionInfoControls(System.Windows.Visibility.Hidden);
                }
            }));
        }
    }
}