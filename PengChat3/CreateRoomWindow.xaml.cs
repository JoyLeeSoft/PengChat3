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
using System.Windows.Shapes;

namespace PengChat3
{
    /// <summary>
    /// CreateRoomWindow.xaml
    /// </summary>
    public partial class CreateRoomWindow : Window
    {
        public string RoomName { get; private set; }

        public short MaxConnectorNum { get; private set; }

        public string Password { get; private set; }

        public CreateRoomWindow()
        {
            InitializeComponent();

            Title = ResourceManager.GetStringByKey("Str_CreateRoom");
            label_Name.Content = ResourceManager.GetStringByKey("Str_RoomName") + " : ";
            label_MaxNum.Content = ResourceManager.GetStringByKey("Str_MaxConnectorNum") + " : ";
            checkBox_MaxNum.Content = ResourceManager.GetStringByKey("Str_NoNeed");
            label_Password.Content = ResourceManager.GetStringByKey("Str_PW") + " : ";
            checkBox_Password.Content = ResourceManager.GetStringByKey("Str_NoNeed");
            textBlock_CreateRoomButton.Text = ResourceManager.GetStringByKey("Str_CreateRoom");
        }

        private void checkBox_MaxNum_CheckedChanged(object sender, RoutedEventArgs e)
        {
            textBox_MaxNum.IsEnabled = (checkBox_MaxNum.IsChecked == false);
        }

        private void checkBox_Password_CheckedChanged(object sender, RoutedEventArgs e)
        {
            textBox_Password.IsEnabled = (checkBox_Password.IsChecked == false);
        }

        private void textBoxes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(null, null);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((textBox_Name.Text != "") && (textBox_MaxNum.Text != "" || checkBox_MaxNum.IsChecked.Value) && 
                (textBox_Password.Text != "" || checkBox_Password.IsChecked.Value))
            {
                try
                {
                    RoomName = textBox_Name.Text;
                    MaxConnectorNum = checkBox_MaxNum.IsChecked.Value ? (short)0 : Convert.ToInt16(textBox_MaxNum.Text);
                    Password = checkBox_Password.IsChecked.Value ? "" : textBox_Password.Text;

                    DialogResult = true;
                    Close();
                }
                catch (Exception ex)
                {
                    Utility.Error(ex.Message);
                    return;
                }
            }
            else
            {
                Utility.Error(ResourceManager.GetStringByKey("Str_EmptyLabel"));
                return;
            }
        }
    }
}
