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
    public partial class MainWindow
    {
        void sock_OnLogin(object sender, LoginEventArgs e)
        {
            switch (e.ErrCode)
            {
                case LoginEventArgs.ErrorCode.Ok:
                    PengChat3ClientSock sock = (PengChat3ClientSock)sender;
                    Logging(ResourceManager.GetStringByKey("Str_SuccessedConnect") + '\n' +
                        string.Format("IP : {0} Port {1}", sock.ConnectedIP, sock.ConnectedPort));
                    break;
                case LoginEventArgs.ErrorCode.UnknownIdPw:
                    string err = ResourceManager.GetStringByKey("Str_NotSuccessedConnect");
                    Logging(err);
                    Utility.Error(err);
                    break;
            }
        }
    }
}