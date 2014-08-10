using System;
using System.Net.Sockets;
using System.Text;

namespace PC3API_dn
{
    public class PengChat3ClientSock : IDisposable
    {
        private static readonly Encoding DefaultEncoding = Encoding.UTF8;
        private static readonly string MagicNumber = Encoding.UTF8.GetString(new byte[] { 0x01, 0x04, 0x03, 0x09 });
        private static readonly byte[] EOP = new byte[1] { (byte)'\0' };

        private TcpClient Client;
        private NetworkStream Stream;
        private bool IsAlreadyDisposed;

        public bool IsConnected { get; private set; }

        public string ConnectedIP { get; private set; }

        public int ConnectedPort { get; private set; }

        public PengChat3ClientSock()
        {
            IsConnected = false;
        }

        ~PengChat3ClientSock()
        {
            Dispose(false);
        }

        public PengChat3ClientSock(string ip, int port, string id, string pw)
        {
            Connect(ip, port, id, pw);
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

        public void Connect(string ip, int port, string id, string pw)
        {
            Client = new TcpClient(ip, port);
            Stream = Client.GetStream();

            IsConnected = true;
            ConnectedIP = ip;
            ConnectedPort = port;

            SendPacket(Protocol.PROTOCOL_CHECK, MagicNumber);
            SendPacket(Protocol.PROTOCOL_LOGIN, id + '\n' + pw);
        }

        private void SendPacket(string header, string data)
        {
            byte[] buf = new byte[header.Length + data.Length + 1];

            buf = Utility.CombineArray(DefaultEncoding.GetBytes(header),
                                       DefaultEncoding.GetBytes(data), 
                                       EOP);

            Stream.Write(buf, 0, buf.Length);
        }
    }
}