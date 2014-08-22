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
        private void ChangeStatusRoomInfoControls(Visibility visibility, RoomListItem r, string master)
        {
            foreach (UIElement ui in grid_GroupBoxRoomInfo.Children)
            {
                if (ui != label_RoomName)
                {
                    ui.Visibility = visibility;
                }
            }

            if (visibility == Visibility.Visible && r != null)
            {
                label_RoomName.Content = r.room.Name;
                label_MasterValue.Content = r.room.Master;
                label_NumValue.Content = (r.room.MaxConnectorNum != 0) ?
                    r.room.MaxConnectorNum.ToString() : ResourceManager.GetStringByKey("Str_Unlimited");
                passwordBox_RoomPW.IsEnabled = r.room.IsNeedPassword;
                button_DeleteRoom.IsEnabled = (r.room.Master == master);
            }
            else
            {
                //listView_RoomInfo.Items.Clear();
                passwordBox_RoomPW.Password = "";
                label_RoomName.Content = ResourceManager.GetStringByKey("Str_NoSelectedRoom");
            }
        }

        private void ChangeStatusConnectionInfoControls(Visibility visibility)
        {
            foreach (UIElement UIElement in grid_ConnectionInfo.Children)
            {
                if (UIElement != comboBox_ConnectionInfo)
                {
                    UIElement.Visibility = visibility;
                }
            }
        }

        private void Logging(string msg)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                textBox_Info.AppendText(msg + "\r\n");
                textBox_Info.ScrollToEnd();
            }));
        }

        private PengChat3ClientSock GetSelectedSock()
        {
            return ((CntComboBoxItem)comboBox_ConnectionInfo.SelectedItem).Sock;
        }

        private RoomListItem GetSelectedRoomItem()
        {
            return (RoomListItem)listView_RoomInfo.SelectedItem;
        }

        private bool IsSelectedSocket(PengChat3ClientSock sock)
        {
            bool b = false;
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate()
            {
                b = (GetSelectedSock() == sock);
            }));

            return b;
        }
    }
}