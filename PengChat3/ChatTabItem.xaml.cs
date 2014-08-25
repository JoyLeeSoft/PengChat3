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
    public class ListViewMemberItem
    {
        public string Nick { get; set; }
        public ImageSource State { get; set; }
    }

    /// <summary>
    /// ChatTabItem.xaml
    /// </summary>
    public partial class ChatTabItem
    {
        internal PengChat3ClientSock Sock = null;
        internal uint RoomID;
        private List<ListViewMemberItem> Members = new List<ListViewMemberItem>();

        private Member.MemberState State = Member.MemberState.Online;

        static BitmapImage OnlineImg, BusyImg;

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
        }

        private void BindingListBox()
        {
            listBox_Members.ItemsSource = new List<ListViewMemberItem>();
            listBox_Members.ItemsSource = Members;
        }

        public ChatTabItem(string headerText, PengChat3ClientSock sock, uint roomid)
        {
            InitializeComponent();
            image_State.Source = OnlineImg;

            textBlock_HeaderText.Text = headerText;
            RoomID = roomid;
            Sock = sock;
        }

        public void AddClient(Member m)
        {
            Members.Add(new ListViewMemberItem() {Nick = m.Nickname, 
                State = (m.State == Member.MemberState.Online) ? OnlineImg : BusyImg});

            BindingListBox();

            //AppendChat(m.Nickname + ' ' + ResourceManager.GetStringByKey("Str_AddClient"));
        }

        public void RemoveClient(string nick)
        {
            Members.RemoveAll(mem => { return mem.Nick == nick; });

            BindingListBox();

            AppendChat(nick + ' ' + ResourceManager.GetStringByKey("Str_RemoveClient"));
        }

        public void ChangeState(string nick, Member.MemberState state)
        {
            Members.Find(m => { return m.Nick == nick; }).State = (state == Member.MemberState.Online) ? OnlineImg : BusyImg;
            
            if (nick == Sock.Nickname)
            {
                image_State.Source = (state == Member.MemberState.Online) ? OnlineImg : BusyImg;
                State = state;
            }

            BindingListBox();
        }

        public void AppendChat(string chat)
        {
            textBox_Chat.AppendText(chat + "\r\n");
        }

        private void textBox_Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AppendChat(Sock.Nickname + " : " + textBox_Input.Text);
                e.Handled = true;
                textBox_Input.Text = "";
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            ExitFromRoom();            
        }

        public void ExitFromRoom()
        {
            if (MessageBox.Show(ResourceManager.GetStringByKey("Str_AreYouSureExit"), "PengChat3", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                Sock.ExitFromRoom(RoomID);
        }

        private void image_State_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Sock.SetMyState(RoomID, State == Member.MemberState.Online ? Member.MemberState.Busy : Member.MemberState.Online);
        }
    }
}