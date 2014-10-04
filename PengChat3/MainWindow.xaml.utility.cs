//#define TEST

using System;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;

using PC3API_dn;

namespace PengChat3
{
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

        internal ViewModel GetViewModelBySocket(PengChat3ClientSock sock)
        {
            ViewModel model = viewModel.Find(m => { return m.Sock == sock; });
            return model;
        }

        internal ChatTab GetChatTabBySocketAndRoomID(PengChat3ClientSock sock, uint roomid)
        {
            foreach (var tab in tabControl_Page.Items)
            {
                if (tab == tabItem_Main)
                    continue;

                ChatTab item = (ChatTab)tab;
                if (item.Sock == sock && item.Room.ID == roomid)
                    return item;
            }

            return null;
        }

        internal void RemoveChatTab(ChatTab tab)
        {
            if (tab != null)
            {
                tabControl_Page.Items.Remove(tab);
                tabControl_Page.SelectedItem = tabItem_Main;
            }
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
            textBlock_CreateRoomButton.Text = ResourceManager.GetStringByKey("Str_CreateRoom");
            textBlock_LogoutButton.Text = ResourceManager.GetStringByKey("Str_Logout");
            textBlock_GroupBoxInfo.Text = ResourceManager.GetStringByKey("Str_InfoWindow");
            #endregion

            #region Data binding
            viewModel = new ObservableCollection<ViewModel>();
            comboBox_CntList.ItemsSource = viewModel;

            LogType.InitLogImages();
            logViewModel = new ObservableCollection<LogType>();
            listView_Log.ItemsSource = logViewModel;
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
