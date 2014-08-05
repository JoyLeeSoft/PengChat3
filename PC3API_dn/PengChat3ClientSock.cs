using System;
using System.Net.Sockets;
using System.Text;

namespace PC3API_dn
{
    public class PengChat3ClientSock : IDisposable
    {
        private readonly Encoding DefaultEncoding = Encoding.UTF8;
        private readonly char ProtocolSeparator = '\0';
        private readonly byte[] MagicNumber = { 0x00, 0x01, 0x00, 0x04 };

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

            //SendPacket(ProtocolHeader.packet_check_real, MagicNumber);
            //SendPacket(ProtocolHeader.packet_login, DefaultEncoding.GetBytes(id + '\n' + pw));
        }

        private void SendPacket(ProtocolHeader header, byte[] data = null)
        {
            byte[] buf;

            if (data != null)
            {
                //buf = Utility.CombineArray(BitConverter.GetBytes((short)header), data, ProtocolSeparator);
            }
            else
            {
                //buf = Utility.CombineArray(BitConverter.GetBytes((short)header), ProtocolSeparator);
            }

            //Stream.Write(buf, 0, buf.Length);
        }
    }
}