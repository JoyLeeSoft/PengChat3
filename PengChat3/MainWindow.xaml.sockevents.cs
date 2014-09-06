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

                sock.GetRoomInfo();
            }
            else
            {
                string err = ResourceManager.GetStringByKey("Str_NotSuccessedConnect") + ' ';

                switch (e.ErrCode)
                {
                    case LoginEventArgs.ErrorCode.UnknownIdPw: 
                        err += ResourceManager.GetStringByKey("Str_PasswordIsWrong");
                        break;
                    case LoginEventArgs.ErrorCode.AlreadyLogged:
                        err += ResourceManager.GetStringByKey("Str_AlreadyLogged");
                        break;
                }

                Dispatcher.Invoke(new Action(delegate()
                {
                    Utility.Error(err);
                }));
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

        private void sock_OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            PengChat3ClientSock sock = (PengChat3ClientSock)sender;

            Dispatcher.Invoke(new Action(delegate()
            {
                viewModel.Remove(s => { return sock == s.Sock; });

                SetSelectedCntItemToEnd();
            }));
        }
    }
}
