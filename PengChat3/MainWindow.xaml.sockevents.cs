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

                try
                {
                    var rooms = GetViewModelBySocket(sock).Rooms;
                    var room = rooms.Find(r => { return r.ID == e.ID.Value; });

                    Dispatcher.Invoke(new Action(delegate()
                    {
                        rooms.Remove(room);

                        RemoveChatTab(GetChatTabBySocketAndRoomID(sock, room.ID));
                    }));

                    if (sock.Nickname == room.Master)
                        Log(LogType.LogKind.Successed, ResourceManager.GetStringByKey("Str_SuccessedToDeleteRoom"));
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void sock_OnAddClient(object sender, AddClientEventArgs e)
        {
            if (e.ErrCode == AddClientEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;
                ChatTab tab;

                if (e.AddedMember.Nickname == sock.Nickname)
                {
                    var room = GetViewModelBySocket(sock).Rooms.Find(r => { return r.ID == e.RoomID.Value; });

                    Dispatcher.Invoke(new Action(delegate()
                    {
                        tab = new ChatTab(sock, room);
                        tabControl_Page.Items.Add(tab);
                        tabControl_Page.SelectedItem = tab;
                        // Of course GetMembersInfo function is also get my info.
                    }));

                    sock.GetMembersInfo(e.RoomID.Value);
                }
                else
                {
                    GetChatTabBySocketAndRoomID(sock, e.RoomID.Value).AddMember(e.AddedMember);
                }                
            }
            else
            {
                string err = ResourceManager.GetStringByKey("Str_CannotEntryToRoom") + '\n';

                switch (e.ErrCode)
                {
                    case AddClientEventArgs.ErrorCode.AlreadyEntered:
                        err += ResourceManager.GetStringByKey("Str_AlreadyLogged");
                        break;
                    case AddClientEventArgs.ErrorCode.PasswordIsWrong:
                        err += ResourceManager.GetStringByKey("Str_PasswordIsWrong");
                        break;
                    case AddClientEventArgs.ErrorCode.RoomIsFull:
                        err += ResourceManager.GetStringByKey("Str_RoomIsFull");
                        break;
                }

                Log(LogType.LogKind.Failed, err);

                Dispatcher.Invoke(new Action(delegate()
                {
                    Utility.Error(err);
                }));
            }
        }

        private void sock_OnGetMembers(object sender, GetMembersEventArgs e)
        {
            if (e.ErrCode == GetMembersEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;

                var tab = GetChatTabBySocketAndRoomID(sock, e.RoomID.Value);

                if (tab != null)
                {
                    Dispatcher.Invoke(new Action(delegate()
                    {
                        foreach (var m in e.Members)
                        {
                            tab.AddMember(m);
                        }
                    }));
                }
            }
        }

        void sock_OnRemoveClient(object sender, RemoveClientEventArgs e)
        {
            if (e.ErrCode == RemoveClientEventArgs.ErrorCode.Ok)
            {
                PengChat3ClientSock sock = (PengChat3ClientSock)sender;

                try
                {
                    var tab = GetChatTabBySocketAndRoomID(sock, e.RoomID.Value);
                    tab.RemoveMember(e.RemovedMemberNickname);

                    if (e.RemovedMemberNickname == sock.Nickname)
                    {
                        var room = GetViewModelBySocket(sock).Rooms.Find(r => { return r.ID == e.RoomID.Value; });

                        Dispatcher.Invoke(new Action(delegate()
                        {
                            RemoveChatTab(tab);
                        }));
                    }
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void sock_OnReceiveChat(object sender, ReceiveChatEventArgs e)
        {
            PengChat3ClientSock sock = (PengChat3ClientSock)sender;

            try
            {
                var tab = GetChatTabBySocketAndRoomID(sock, e.RoomID.Value);

                tab.AppendChat(e.Sender, e.Message);
            }
            catch (NullReferenceException)
            {

            }
        }
    }
}
