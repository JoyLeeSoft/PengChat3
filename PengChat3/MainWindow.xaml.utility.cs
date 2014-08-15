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

namespace PengChat3
{
    public partial class MainWindow
    {
        private void ChangeStatusRoomInfoControls(Visibility visibility, RoomListItem r)
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
                image_IsNeedPWValue.Source = r.room.IsNeedPassword ? YesImage : NoImage;
            }
            else
            {
                listView_RoomInfo.Items.Clear();
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

        private BitmapImage YesImage, NoImage;

        private void InitImages()
        {
            YesImage = new BitmapImage();
            YesImage.BeginInit();
            YesImage.UriSource = new Uri(@"Resources\yes.png", UriKind.Relative);
            YesImage.EndInit();

            NoImage = new BitmapImage();
            NoImage.BeginInit();
            NoImage.UriSource = new Uri(@"Resources\no.png", UriKind.Relative);
            NoImage.EndInit();
        }

        private void Logging(string msg)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                textBox_Info.AppendText(msg + "\r\n");
                textBox_Info.ScrollToEnd();
            }));
        }
    }
}