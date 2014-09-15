using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PC3API_dn;

namespace PengChat3
{
    public partial class MainWindow
    {
        private void sock_OnLogin(object sender, LoginEventArgs e)
        {
            if (e.ErrCode == LoginEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;

                Dispatcher.Invoke(new Action(delegate()
                {
                    ViewModel model = new ViewModel(sock);

                    viewModel.Add(model);
                    
                    SetSelectedCntItemToEnd();
                }));

                Log(LogType.LogKind.Successed, ResourceManager.GetStringByKey("Str_SuccessedConnect") + 
                    '\n' + e.ConnectedIP + ':' + e.ConnectedPort);

                sock.GetRoomInfo();
            }
            else
            {
                string err = ResourceManager.GetStringByKey("Str_NotSuccessedConnect") + '\n';

                switch (e.ErrCode)
                {
                    case LoginEventArgs.ErrorCode.UnknownIdPw: 
                        err += ResourceManager.GetStringByKey("Str_PasswordIsWrong");
                        break;
                    case LoginEventArgs.ErrorCode.AlreadyLogged:
                        err += ResourceManager.GetStringByKey("Str_AlreadyLogged");
                        break;
                }

                Log(LogType.LogKind.Failed, err);
                Dispatcher.Invoke(new Action(delegate()
                {
                    Utility.Error(err);
                }));
            }
        }

        private void sock_OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            PengChat3ClientSock sock = (PengChat3ClientSock)sender;

            Dispatcher.Invoke(new Action(delegate()
            {
                viewModel.Remove(s => { return sock == s.Sock; });

                SetSelectedCntItemToEnd();
            }));

            string temp = '\n' + e.DisconnectedIP + ':' + e.DisconnectedPort;

            switch (e.ErrCode)
            {
                case DisconnectedEventArgs.ErrorCode.Logout:
                    Log(LogType.LogKind.Successed, ResourceManager.GetStringByKey("Str_LoggedOut") + temp);
                    break;
                case DisconnectedEventArgs.ErrorCode.ServerError:
                    string err = ResourceManager.GetStringByKey("Str_DisconnectedServer") + temp;
                    Log(LogType.LogKind.Failed, err);
                    break;
            }
        }

        private void sock_OnRoomInfo(object sender, RoomInfoEventArgs e)
        {
            PengChat3ClientSock sock = (PengChat3ClientSock)sender;

            Dispatcher.Invoke(new Action(delegate()
            {
                var item = viewModel.Find(s => { return sock == s.Sock; });

                item.Rooms.Clear();

                foreach (var room in e.Rooms)
                {
                    item.Rooms.Add(room);
                }
            }));
        }

        private void sock_OnCreateRoom(object sender, CreateRoomEventArgs e)
        {
            if (e.ErrCode == CreateRoomEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;

                Dispatcher.Invoke(new Action(delegate()
                {
                    GetViewModelBySocket(sock).Rooms.Add(e.NewRoom);
                }));

                if (sock.Nickname == e.NewRoom.Master)
                    Log(LogType.LogKind.Successed, ResourceManager.GetStringByKey("Str_SuccessedToCreateRoom"));
            }
            else
            {
                string err = ResourceManager.GetStringByKey("Str_FailedToCreateRoom") + '\n';
                switch (e.ErrCode)
                {
                    case CreateRoomEventArgs.ErrorCode.RoomNameOverlap:
                        err += ResourceManager.GetStringByKey("Str_RoomNameOverlap");
                        break;
                }

                Log(LogType.LogKind.Failed, err);

                Dispatcher.Invoke(new Action(delegate()
                {
                    Utility.Error(err);
                }));
            }
        }

        private void sock_OnRemoveRoom(object sender, RemoveRoomEventArgs e)
        {
            if (e.ErrCode == RemoveRoomEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;
                var rooms = GetViewModelBySocket(sock).Rooms;
                var room = rooms.Find(r => { return r.ID == e.ID.Value; });

                Dispatcher.Invoke(new Action(delegate()
                {
                    rooms.Remove(room);
                }));

                if (sock.Nickname == room.Master)
                    Log(LogType.LogKind.Successed, ResourceManager.GetStringByKey("Str_SuccessedToDeleteRoom"));
            }
        }

        private void sock_OnAddClient(object sender, AddClientEventArgs e)
        {
            if (e.ErrCode == AddClientEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;

                if (e.AddedMember.Nickname == sock.Nickname)
                {
                    var room = GetViewModelBySocket(sock).Rooms.Find(r => { return r.ID == e.RoomID.Value; });

                    Dispatcher.Invoke(new Action(delegate()
                    {
                        ChatTab tab = new ChatTab(sock, room);
                        tabControl_Page.Items.Add(tab);
                        tabControl_Page.SelectedItem = tab;
                    }));
                }
                else
                {

                }
            }
        }
    }
}
