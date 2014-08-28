using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PC3API_dn
{
    public partial class PengChat3ClientSock : IDisposable
    {
        private static readonly int MAX_BYTES_NUMBER = 1024;
        private static readonly int PACKET_HEADER_SIZE = 4;
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private static readonly byte[] MagicNumber = new byte[] { 0x01, 0x04, 0x03, 0x09 };
        private static readonly byte EOP = (byte)'\0';
        public const short Capacity_Unlimited = (short)0;
        public const string Password_NotUsed = "\a";

        private TcpClient Client = null;
        private NetworkStream Stream = null;
        private bool IsAlreadyDisposed = false;
        private Thread RecvThread = null;

        public bool IsConnected { get; private set; }

        public bool IsLogged { get; private set; }

        public string ConnectedIP { get; private set; }

        public int ConnectedPort { get; private set; }

        public string Nickname { get; private set; }

        private List<Room> Rooms_ = new List<Room>();

        public Room[] Rooms { get { return Rooms_.ToArray(); } }

        public PengChat3ClientSock()
        {
            IsConnected = false;
            IsLogged = false;
            ConnectedIP = null;
            ConnectedPort = 0;
            Nickname = null;
        }

        ~PengChat3ClientSock()
        {
            Dispose(false);
        }

        public PengChat3ClientSock(string ip, int port, string id, string pw)
        {
            Connect(ip, port);
            Login(id, pw);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool bManaged)
        {
            if (IsAlreadyDisposed)
                return;

            if (bManaged)
            {
                // Delete managed resources
            }

            // Delete unmanaged resources

            if (IsLogged)
            {
                Logout();
            }

            if (RecvThread != null)
            {
                IsNormalClose = true;
                Stream.Close();
                RecvThread.Join();
                RecvThread = null;
            }
            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }
            if (Client != null)
            {
                Client.Close();
                Client = null;
            }

            IsAlreadyDisposed = true;
        }

        public void Connect(string ip, int port)
        {
            Client = new TcpClient(ip, port);
            Stream = Client.GetStream();

            IsConnected = true;
            ConnectedIP = ip;
            ConnectedPort = port;

            RecvThread = new Thread(new ThreadStart(RecvThreadFunc));
            RecvThread.Start();
        }

        public void Login(string id, string pw)
        {
            SendPacket(Protocol.PROTOCOL_CHECK_REAL, DefaultEncoding.GetString(MagicNumber));
            SendPacket(Protocol.PROTOCOL_LOGIN, id + '\n' + pw);
        }

        public void Logout()
        {
            IsLogged = false;

            foreach (Room rm in Rooms_)
            {
                Member mem = rm.Members_.Find(m => { return m.Nickname == Nickname; });

                if (mem != null)
                {
                    if (OnRemoveClient != null)
                    {
                        OnRemoveClient(this, new RemoveClientEventArgs(RemoveClientEventArgs.ErrorCode.Ok, rm.ID, mem.Nickname));
                    }
                } 
            }

            Dispose();

            if (OnDisconnected != null)
            {
                OnDisconnected(this, new DisconnectedEventArgs(ConnectedIP, ConnectedPort,
                    DisconnectedEventArgs.ErrorCode.Logout));
            }
        }

        public void GetRoomInfo()
        {
            SendPacket(Protocol.PROTOCOL_GET_ROOM_INFO);
        }

        public void CreateRoom(string name, short capacity = Capacity_Unlimited, string password = Password_NotUsed)
        {
            SendPacket(Protocol.PROTOCOL_CREATE_ROOM, name + '\n' + capacity.ToString() + '\n' + password);
        }

        public void DeleteRoom(uint id)
        {
            SendPacket(Protocol.PROTOCOL_REMOVE_ROOM, id.ToString());
        }

        public void EntryToRoom(uint id, string password = Password_NotUsed)
        {
            SendPacket(Protocol.PROTOCOL_ADD_CLIENT, id.ToString() + '\n' + password);
        }

        public void ExitFromRoom(uint id)
        {
            SendPacket(Protocol.PROTOCOL_REMOVE_CLIENT, id.ToString());
        }

        public void GetMembers(uint id)
        {
            SendPacket(Protocol.PROTOCOL_GET_MEMBERS, id.ToString());
        }

        public void SetMyState(uint id, Member.MemberState state)
        {
            SendPacket(Protocol.PROTOCOL_CHANGE_STATE, id.ToString() + '\n' + ((byte)state).ToString());
        }

        private void SendPacket(string header, string data = "")
        {
            byte[] buf = new byte[1024];

            buf = Utility.CombineArray(DefaultEncoding.GetBytes(header),
                                       DefaultEncoding.GetBytes(data),
                                       new byte[1] { EOP });

            Stream.Write(buf, 0, buf.Length);
        }
    }
}