using System;
using System.Collections.Generic;
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
using System.Collections.ObjectModel;
using System.Globalization;

using PC3API_dn;

namespace PengChat3
{
    [ValueConversion(typeof(Member.MemberState), typeof(ImageSource))]
    public class StateImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            switch ((Member.MemberState)value)
            {
                case Member.MemberState.Online: return ChatTab.OnlineImg;
                case Member.MemberState.Busy: return ChatTab.BusyImg;
                default: return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack");
        }
    }

    /// <summary>
    /// Interaction logic for ChatTab.xaml
    /// </summary>
    public partial class ChatTab : MahApps.Metro.Controls.MetroTabItem
    {
        internal static BitmapImage OnlineImg, BusyImg, MasterImg;

        internal static void InitImages()
        {
            OnlineImg = new BitmapImage();
            OnlineImg.BeginInit();
            OnlineImg.UriSource = new Uri(@"Resources\bullet-green.png", UriKind.Relative);
            OnlineImg.EndInit();

            BusyImg = new BitmapImage();
            BusyImg.BeginInit();
            BusyImg.UriSource = new Uri(@"Resources\bullet-red.png", UriKind.Relative);
            BusyImg.EndInit();

            MasterImg = new BitmapImage();
            MasterImg.BeginInit();
            MasterImg.UriSource = new Uri(@"Resources\star.png", UriKind.Relative);
            MasterImg.EndInit();
        }

        public ObservableCollection<Member> Members { get; private set; }

        internal PengChat3ClientSock Sock;
        internal Room Room;

        public ChatTab(PengChat3ClientSock sock, Room room)
        {
            InitializeComponent();

            Sock = sock;
            Room = room;

            Header = Room.Name;

            Members = new ObservableCollection<Member>();
            listView_Members.ItemsSource = Members;

            button_Exit.Content = ResourceManager.GetStringByKey("Str_Exit");
        }

        public void CloseChat()
        {
            Sock.ExitFromRoom(Room.ID);
        }

        private void button_Exit_Click(object sender, RoutedEventArgs e)
        {
            CloseChat();
        }

        public void AddMember(Member newMem)
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                Members.Add(newMem);
            }));
        }

        public void RemoveMember(string revMem)
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                Members.Remove(m => { return m.Nickname == revMem; });
            }));
        }

        public void AppendChat(string sender, string chat)
        {
            Dispatcher.Invoke(new Action(delegate()
            {
                textBox_View.AppendText(sender + " : " + chat + '\n');
                textBox_View.ScrollToEnd();
            }));
        }

        private void textBox_Chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (textBox_Chat.Text != "" && e.Key == Key.Enter)
            {
                Sock.SendChat(Room.ID, Sock.Nickname, textBox_Chat.Text);
                textBox_Chat.Clear();
            }
        }
    }
}
