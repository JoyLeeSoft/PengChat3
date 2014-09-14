using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

using PC3API_dn;

namespace PengChat3
{
    public class ViewModel
    {
        public PengChat3ClientSock Sock { get; internal set; }

        public ObservableCollection<Room> Rooms { get; internal set; }

        public ViewModel(PengChat3ClientSock sock)
        {
            Sock = sock;
            Rooms = new ObservableCollection<Room>();
        }
    }

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
    public class DeleteButtonVisibilityConverter : IValueConverter
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
    public class VisibilityConverter : IValueConverter
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

    [ValueConversion(typeof(String), typeof(String))]
    public class EntryButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ResourceManager.GetStringByKey("Str_Entry");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    [ValueConversion(typeof(String), typeof(String))]
    public class DeleteButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ResourceManager.GetStringByKey("Str_Delete");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    //[ValueConversion(typeof(UInt32), typeof(String))]
    public class EntryButtonTagConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string[] result = Array.ConvertAll<object, string>(values, obj =>
            {
                return (obj == null) ? string.Empty : obj.ToString();
            });

            return result[0] + '\n' + result[1] + '\n' + result[2];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    public partial class MainWindow
    {
        public ObservableCollection<ViewModel> viewModel { get; set; }
    }
}
