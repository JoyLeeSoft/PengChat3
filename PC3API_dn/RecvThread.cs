using System;
using System.Collections.Generic;

namespace PC3API_dn
{
    public partial class PengChat3ClientSock
    {
        private bool CheckSuccessed(string pack)
        {
            return pack[0] == Protocol.FLAG_SUCCESSED;
        }

        private string DeleteErrorCode(string pack)
        {
            return pack.Remove(0, 1);
        }

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
                OnLoginResult(CheckSuccessed(pack), DeleteErrorCode(pack));
            else if (header == Protocol.PROTOCOL_GET_ROOM_INFO)
                OnGetRoomInfoResult(pack);
            else if (header == Protocol.PROTOCOL_CREATE_ROOM)
                OnAddRoomResult(CheckSuccessed(pack), DeleteErrorCode(pack));
            else if (header == Protocol.PROTOCOL_REMOVE_ROOM)
                OnRemoveRoomResult(CheckSuccessed(pack), DeleteErrorCode(pack));
            else if (header == Protocol.PROTOCOL_ADD_CLIENT)
                OnAddClientResult(CheckSuccessed(pack), DeleteErrorCode(pack));
            else if (header == Protocol.PROTOCOL_REMOVE_CLIENT)
                OnRemoveClientResult(CheckSuccessed(pack), DeleteErrorCode(pack));
            else if (header == Protocol.PROTOCOL_GET_MEMBERS)
                OnGetMembersResult(CheckSuccessed(pack), DeleteErrorCode(pack));
            else if (header == Protocol.PROTOCOL_CHANGE_STATE)
                OnChangeStateResult(pack[0] == '1', pack.Remove(0, 1));
        }

        private void OnLoginResult(bool successed, string pack)
        {
            if (successed)
            {
                IsLogged = true;
                Nickname = pack;

                if (OnLogin != null)
                    OnLogin(this, new LoginEventArgs(ConnectedIP, ConnectedPort, Nickname, LoginEventArgs.ErrorCode.Ok));
            }
            else
            {
                if (OnLogin != null)
                    OnLogin(this, new LoginEventArgs(null, 0, null, (LoginEventArgs.ErrorCode)Convert.ToByte(pack)));
            }
        }
        
        private void OnGetRoomInfoResult(string pack)
        {
            Rooms_.Clear();

            if (pack != "")
            {
                foreach (var temp in pack.Split('\n'))
                {
                    Room one_room = Room.ToRoom(temp);
                    Rooms_.Add(one_room);
                }
            }

            OnRoomInfo(this, new RoomInfoEventArgs(Rooms_.ToArray()));
        }

        private void OnAddRoomResult(bool successed, string pack)
        {
            if (successed)
            {
                Room one_room = Room.ToRoom(pack);
                Rooms_.Add(one_room);

                if (OnCreateRoom != null)
                {
                    OnCreateRoom(this, new CreateRoomEventArgs(CreateRoomEventArgs.ErrorCode.Ok, one_room));
                }
            }
            else
            {
                if (OnCreateRoom != null)
                {
                    OnCreateRoom(this, new CreateRoomEventArgs((CreateRoomEventArgs.ErrorCode)Convert.ToByte(pack), null));
                }
            }
        }

        private void OnRemoveRoomResult(bool successed, string pack)
        {
            if (successed)
            {
                uint id = Convert.ToUInt32(pack);

                Rooms_.RemoveAll(r => { return r.ID == id; });

                if (OnRemoveRoom != null)
                {
                    OnRemoveRoom(this, new RemoveRoomEventArgs(RemoveRoomEventArgs.ErrorCode.Ok, id));
                }
            }
            else
            {
                if (OnRemoveRoom != null)
                {
                    OnRemoveRoom(this, new RemoveRoomEventArgs((RemoveRoomEventArgs.ErrorCode)Convert.ToByte(pack), null));
                }
            }
        }

        private void OnAddClientResult(bool successed, string pack)
        {
            if (successed)
            {
                string[] id_member = pack.Split('\n');
                uint room_id = Convert.ToUInt32(id_member[0]);

                Room rm = Rooms_.Find(r => { return r.ID == room_id; } );

                Member added_member = Member.ToMember(id_member[1]);
                rm.Members_.Add(added_member);

                if (OnAddClient != null)
                    OnAddClient(this, new AddClientEventArgs(AddClientEventArgs.ErrorCode.Ok, room_id, added_member));
            }
            else
            {
                if (OnAddClient != null)
                    OnAddClient(this, new AddClientEventArgs((AddClientEventArgs.ErrorCode)Convert.ToByte(pack), 
                        null, null));
            }
        }

        private void OnRemoveClientResult(bool successed, string pack)
        {
            if (successed)
            {
                string[] id_nick = pack.Split('\n');
                uint room_id = Convert.ToUInt32(id_nick[0]);

                Room rm = Rooms_.Find(r => { return r.ID == room_id; });

                rm.Members_.RemoveAll(m => { return m.Nickname == id_nick[1]; });

                if (OnRemoveClient != null)
                {
                    OnRemoveClient(this, new RemoveClientEventArgs(RemoveClientEventArgs.ErrorCode.Ok, room_id, id_nick[1]));
                }
            }
            else
            {
                if (OnRemoveClient != null)
                {
                    OnRemoveClient(this, new RemoveClientEventArgs((RemoveClientEventArgs.ErrorCode)Convert.ToByte(pack),
                        null, null));
                }
            }
        }

        private void OnGetMembersResult(bool successed, string pack)
        {
            if (successed)
            {
                string[] temp = pack.Split('\n');

                uint room_id = Convert.ToUInt32(temp[0]);
                string[] temp2 = temp[1].Split('\a');

                Room rm = Rooms_.Find(r => { return r.ID == room_id; });
                List<Member> tempMembers = new List<Member>();

                foreach (string s in temp2)
                {
                    Member m = Member.ToMember(s);

                    rm.Members_.Add(m);
                    tempMembers.Add(m);
                }

                if (OnGetMembers != null)
                {
                    OnGetMembers(this, new GetMembersEventArgs(GetMembersEventArgs.ErrorCode.Ok, room_id,
                        tempMembers.ToArray()));
                }
            }
            else
            {
                if (OnGetMembers != null)
                {
                    OnGetMembers(this, new GetMembersEventArgs((GetMembersEventArgs.ErrorCode)Convert.ToByte(pack), null, null));
                }
            }
        }

        private void OnChangeStateResult(bool successed, string pack)
        {
            if (successed)
            {
                string[] temp = pack.Split('\n');

                uint room_id = Convert.ToUInt32(temp[0]);
                string nick = temp[1];
                Member.MemberState state = (Member.MemberState)Convert.ToByte(temp[2]);

                Room rm = Rooms_.Find(r => { return r.ID == room_id; });
                rm.Members_.Find(m => { return m.Nickname == nick; }).State = state;


                if (OnChangeState != null)
                {
                    OnChangeState(this, new ChangeStateEventArgs(ChangeStateEventArgs.ErrorCode.Ok, room_id, nick, state));
                }
            }
            else
            {
                if (OnChangeState != null)
                {
                    OnChangeState(this, new ChangeStateEventArgs((ChangeStateEventArgs.ErrorCode)Convert.ToByte(pack), 
                        null, null, null));
                }
            }
        }
    }
}