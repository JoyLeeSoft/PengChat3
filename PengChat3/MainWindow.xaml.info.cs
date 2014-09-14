using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace PengChat3
{
    public class LogType
    {
        public static BitmapImage SuccessedImg, FailedImg, WarningImg;

        public static void InitLogImages()
        {
            SuccessedImg = new BitmapImage();
            SuccessedImg.BeginInit();
            SuccessedImg.UriSource = new Uri(@"Resources\successed.png", UriKind.Relative);
            SuccessedImg.EndInit();

            FailedImg = new BitmapImage();
            FailedImg.BeginInit();
            FailedImg.UriSource = new Uri(@"Resources\failed.png", UriKind.Relative);
            FailedImg.EndInit();

            WarningImg = new BitmapImage();
            WarningImg.BeginInit();
            WarningImg.UriSource = new Uri(@"Resources\warning.png", UriKind.Relative);
            WarningImg.EndInit();
        }

        public enum LogKind
        {
            Successed,
            Failed,
            Warning,
        }

        public LogKind Kind { get; private set; }

        public string Message { get; private set; }

        public LogType(LogKind kind, string msg)
        {
            Kind = kind;
            Message = msg;
        }
    }

    public class LogTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            LogType.LogKind log = (LogType.LogKind)value;

            switch (log)
            {
                case LogType.LogKind.Successed:
                    return LogType.SuccessedImg;
                case LogType.LogKind.Failed:
                    return LogType.FailedImg;
                case LogType.LogKind.Warning:
                    return LogType.WarningImg;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    public partial class MainWindow
    {
        public ObservableCollection<LogType> logViewModel { get; set; }

        public void Log(LogType.LogKind kind, string message)
        {
            LogType log = new LogType(kind, message);

            Dispatcher.Invoke(new Action(delegate()
            {
                logViewModel.Add(log);
                listView_Log.ScrollIntoView(log);
            }));
        }
    }
}
