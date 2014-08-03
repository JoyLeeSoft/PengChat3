using System;
using System.Net.Sockets;

namespace PCAPI_dn
{
    public class PengChat3ClientSock : IDisposable
    {
        private TcpClient Client;
        private NetworkStream Stream;
        private bool IsAlreadyDisposed;

        public bool IsConnected { get; private set; }

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

        void Connect(string ip, int port, string id, string pw)
        {
            Client.Connect(ip, port);

            IsConnected = true;
        }
    }
}
