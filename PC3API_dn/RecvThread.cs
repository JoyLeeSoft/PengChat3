using System;
using System.Collections.Generic;

namespace PC3API_dn
{
    public partial class PengChat3ClientSock
    {
        private bool IsNormalClose = false;

        private void RecvThreadFunc()
        {
            List<byte> buf = new List<byte>(new byte[MAX_BYTES_NUMBER]);
            List<byte> real_buf = new List<byte>(new byte[MAX_BYTES_NUMBER + 1]);
            int i = 0, j = 0, packet_size = 0;

            while (true)
            {
                int read_bytes = 0;

                try
                {
                    byte[] temp_buf = new byte[MAX_BYTES_NUMBER];
                    read_bytes = Stream.Read(temp_buf, 0, MAX_BYTES_NUMBER);
                    buf.InsertRange(0, temp_buf);
                }
                catch (Exception)
                {
                    goto delete_client;
                }

                if (read_bytes <= 0)
                    goto delete_client;

                buf.RemoveRange(read_bytes, MAX_BYTES_NUMBER - read_bytes);

                for (i = 0; i < read_bytes; i++)
                {
                    real_buf[i] = buf[i];
                    packet_size++;

                    if (buf[i] == EOP)
                    {
                        byte[] tmp_str = new byte[packet_size - 1];
                        Array.Copy(real_buf.ToArray(), tmp_str, packet_size - 1);

                        PacketProcessor(DefaultEncoding.GetString(tmp_str));

                        j = 0;
                        packet_size = 0;
                    }
                    else
                    {
                        j++;
                    }
                }
                if (j != 0)
                {
                    real_buf[j] = EOP;

                    byte[] tmp_str = new byte[j - 1];
                    Array.Copy(real_buf.ToArray(), tmp_str, j - 1);

                    PacketProcessor(DefaultEncoding.GetString(tmp_str));
                }

            }

        delete_client:
            if (IsNormalClose == false)
            {
                if (OnDisconnected != null)
                {
                    OnDisconnected(this, new DisconnectedEventArgs(ConnectedIP, ConnectedPort, 
                        DisconnectedEventArgs.ErrorCode.ServerError));
                }
            }
        }

        private void PacketProcessor(string pack)
        {
            // If it is not real packet
            if (pack.Length < PACKET_HEADER_SIZE)
                return;

            string header = pack.Substring(0, PACKET_HEADER_SIZE);
            pack = pack.Remove(0, PACKET_HEADER_SIZE);

            if (header == Protocol.PROTOCOL_LOGIN)
                OnLoginResult(pack);
            else if (header == Protocol.PROTOCOL_GET_ROOM_INFO)
                OnGetRoomInfoResult(pack);
        }

        private void OnLoginResult(string pack)
        {
            if (pack != "")
            {
                IsLogged = true;
                Nickname = pack;

                if (OnLogin != null)
                    OnLogin(this, new LoginEventArgs(ConnectedIP, ConnectedPort, Nickname, LoginEventArgs.ErrorCode.Ok));
            }
            else
            {
                if (OnLogin != null)
                    OnLogin(this, new LoginEventArgs(null, 0, null, LoginEventArgs.ErrorCode.UnknownIdPw));
            }
        }
        
        private void OnGetRoomInfoResult(string pack)
        {

        }
    }
}