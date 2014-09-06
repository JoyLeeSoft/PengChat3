#define TEST

using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;

using PC3API_dn;

namespace PengChat3
{
    [ValueConversion(typeof(Int16), typeof(String))]
    public class MaxConnectorNumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";

            if (((Int16)value) != 0)
                return value.ToString();
            else
                return ResourceManager.GetStringByKey("Str_Unlimited");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    [ValueConversion(typeof(String), typeof(Boolean))]
    public class DeleteButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            PengChat3ClientSock SelectedSock = App.Instance.GetSelectedSock();

            if (SelectedSock != null)
            {
                if (SelectedSock.Nickname == (string)value)
                    return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    [ValueConversion(typeof(Boolean), typeof(Visibility))]
    public class ListViewVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;

            if ((Boolean)value)
                return Visibility.Hidden;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    public partial class MainWindow
    {
        internal ViewModel GetSelectedViewModel()
        {
            if (comboBox_CntList.SelectedItem != null)
                return comboBox_CntList.SelectedItem as ViewModel;
            else
                return null;
        }

        internal PengChat3ClientSock GetSelectedSock()
        {
            ViewModel model = GetSelectedViewModel();

            if (model != null)
                return model.Sock;
            else
                return null;
        }

        private void SetSelectedCntItemToEnd()
        {            
            if (comboBox_CntList.Items.IsEmpty)
                comboBox_CntList.SelectedItem = null;
            else
                comboBox_CntList.SelectedIndex = comboBox_CntList.Items.Count - 1;
        }

        private void InitializeSettings()
        {
            #region Control name settings
            
            tabItem_Main.Header = ResourceManager.GetStringByKey("Str_MainPage");
            menuItem_File.Header = ResourceManager.GetStringByKey("Str_File");
            menuItem_TabClose.Header = ResourceManager.GetStringByKey("Str_TabClose");
            menuItem_Exit.Header = ResourceManager.GetStringByKey("Str_Exit");
            textBlock_groupBoxLogin.Text = ResourceManager.GetStringByKey("Str_Login");
            label_ID.Content = ResourceManager.GetStringByKey("Str_ID") + " : ";
            label_PW.Content = ResourceManager.GetStringByKey("Str_PW") + " : ";
            label_IP.Content = ResourceManager.GetStringByKey("Str_IP") + " : ";
            textBlock_LoginButton.Text = ResourceManager.GetStringByKey("Str_Login");
            textBlock_groupBoxConnectionInfo.Text = ResourceManager.GetStringByKey("Str_ConnectionInfo");
            gridViewColumn_RoomName.Header = ResourceManager.GetStringByKey("Str_RoomName");
            gridViewColumn_Master.Header = ResourceManager.GetStringByKey("Str_Master");
            gridViewColumn_MaxConnectorNum.Header = ResourceManager.GetStringByKey("Str_MaxConnectorNum");
            gridViewColumn_IsNeedPassword.Header = ResourceManager.GetStringByKey("Str_PW");
            
            #endregion

            #region Data binding
            viewModel = new ObservableCollection<ViewModel>();
            comboBox_CntList.ItemsSource = viewModel;
            #endregion

            textBox_ID.Focus();

#if TEST
            textBox_ID.Text = "1";
            textBox_PW.Password = "1";
            textBox_IP.Text = "127.0.0.1";
#endif
        }
    }
}
