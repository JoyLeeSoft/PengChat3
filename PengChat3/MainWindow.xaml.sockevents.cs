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

                    PengChat3ClientSock sock = (PengChat3ClientSock)sender;

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        CntComboBoxItem item = new CntComboBoxItem();
                        item.Text = e.ConnectedIP + ":" + e.ConnectedPort.ToString();
                        item.Sock = sock;

                        comboBox_ConnectionInfo.Items.Add(item);
                        comboBox_ConnectionInfo.SelectedItem = item;

                        ChangeStatusConnectionInfoControls(Visibility.Visible);
                    }));

                    sock.GetRoomInfo();
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
            string err;

            switch (e.ErrCode)
            {
                case DisconnectedEventArgs.ErrorCode.ServerError:
                    err = ResourceManager.GetStringByKey("Str_DisconnectedServer") + '\n' 
                    + e.DisconnectedIP + ':' + e.DisconnectedPort.ToString();
                    Logging(err);
                    break;

                case DisconnectedEventArgs.ErrorCode.Logout:
                    err = ResourceManager.GetStringByKey("Str_LoggedOut") + '\n' 
                    + e.DisconnectedIP + ':' + e.DisconnectedPort.ToString();
                    Logging(err);
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
                    ChangeStatusConnectionInfoControls(Visibility.Hidden);
                }
                else
                {
                    comboBox_ConnectionInfo.SelectedIndex = comboBox_ConnectionInfo.Items.Count - 1;
                }
            }));
        }

        void sock_OnRoomInfo(object sender, RoomInfoEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                foreach (var r in e.Rooms)
                {
                    listView_RoomInfo.Items.Add(new RoomListItem(r));
                }
            }));
        }

        void sock_OnCreateRoom(object sender, CreateRoomEventArgs e)
        {
            if (e.ErrCode == CreateRoomEventArgs.ErrorCode.Ok)
            {
                if (IsSelectedSocket((PengChat3ClientSock)sender))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            RoomListItem item = new RoomListItem(e.NewRoom);
                            listView_RoomInfo.Items.Add(item);
                            listView_RoomInfo.SelectedItem = item;
                        }));
                    Logging(ResourceManager.GetStringByKey("Str_SuccessedToCreateRoom"));
                }
            }
            else
            {
                string s = ResourceManager.GetStringByKey("Str_FailedToCreateRoom") + ' ';

                switch (e.ErrCode)
                {
                    case CreateRoomEventArgs.ErrorCode.UnknownCapacity:
                        s += ResourceManager.GetStringByKey("Str_UnknownCapacity");
                        break;
                    case CreateRoomEventArgs.ErrorCode.RoomNameOverlap:
                        s += ResourceManager.GetStringByKey("Str_RoomNameOverlap");
                        break;
                }

                Logging(s);
                Utility.Error(s);
            }
        }

        void sock_OnDeleteRoom(object sender, DeleteRoomEventArgs e)
        {
            if (e.ErrCode == DeleteRoomEventArgs.ErrorCode.Ok)
            {
                if (IsSelectedSocket((PengChat3ClientSock)sender))
                {
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        var r = GetSelectedRoomItem();

                        if (r != null)
                        {
                            listView_RoomInfo.Items.Remove(r);
                        }

                        if (listView_RoomInfo.Items.IsEmpty)
                        {
                            ChangeStatusRoomInfoControls(Visibility.Hidden, null, null);
                        }
                        else
                        {
                            listView_RoomInfo.SelectedIndex = listView_RoomInfo.Items.Count - 1;
                        }
                    }));

                    Logging(ResourceManager.GetStringByKey("Str_SuccessedToDeleteRoom"));
                }
            }
            else
            {
                string s = ResourceManager.GetStringByKey("Str_FailedToDeleteRoom") + ' ';

                switch (e.ErrCode)
                {
                    case DeleteRoomEventArgs.ErrorCode.UnknownID:
                        s += ResourceManager.GetStringByKey("Str_UnknownRoomID");
                        break;
                    case DeleteRoomEventArgs.ErrorCode.RoomNotExist:
                        s += ResourceManager.GetStringByKey("Str_RoomNotExist");
                        break;
                    case DeleteRoomEventArgs.ErrorCode.AccessDenied:
                        s += ResourceManager.GetStringByKey("Str_AccessDenied");
                        break;
                }

                Logging(s);
                Utility.Error(s);
            }
        }
    }
}