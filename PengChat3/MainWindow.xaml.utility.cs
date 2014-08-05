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

namespace PengChat3
{
    public partial class MainWindow
    {
        private void ChangeStatusRoomInfoControls(System.Windows.Visibility visibility)
        {
            foreach (UIElement UIElement in grid_GroupBoxRoomInfo.Children)
            {
                if (UIElement != label_RoomName)
                {
                    UIElement.Visibility = visibility;
                }
            }
        }

        private void ChangeStatusConnectionInfoControls(System.Windows.Visibility visibility)
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
            YesImage.UriSource = new Uri(@"Resources\bullet-blue.png", UriKind.Relative);
            YesImage.EndInit();

            NoImage = new BitmapImage();
            NoImage.BeginInit();
            NoImage.UriSource = new Uri(@"Resources\bullet-blue.png", UriKind.Relative);
            NoImage.EndInit();
        }
    }
}