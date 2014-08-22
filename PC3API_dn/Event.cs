using System;

namespace PC3API_dn
{
    public class LoginEventArgs : EventArgs
    {
        public string ConnectedIP { get; private set; }
        public int ConnectedPort { get; private set; }

        public string Nickname { get; private set; }

        public enum ErrorCode
        {
            Ok,
            UnknownIdPw,
        }

        public ErrorCode ErrCode { get; private set; }

        public LoginEventArgs(string ip, int port, string nickname, ErrorCode errcode)
        {
            ConnectedIP = ip;
            ConnectedPort = port;
            Nickname = nickname;
            ErrCode = errcode;
        }
    }
    public delegate void OnLoginDele(object sender, LoginEventArgs e);

    public class DisconnectedEventArgs : EventArgs
    {
        public string DisconnectedIP { get; private set; }
        public int DisconnectedPort { get; private set; }

        public enum ErrorCode
        {
            Logout,
            ServerError,
        }

        public ErrorCode ErrCode { get; private set; }

        public DisconnectedEventArgs(string ip, int port, ErrorCode errcode)
        {
            DisconnectedIP = ip;
            DisconnectedPort = port;
            ErrCode = errcode;
        }
    }
    public delegate void OnDisconnectedDele(object sender, DisconnectedEventArgs e);

    public class RoomInfoEventArgs : EventArgs
    {
        public Room[] Rooms { get; private set; }

        public RoomInfoEventArgs(Room[] rooms)
        {
            Rooms = rooms;
        }
    }
    public delegate void OnRoomInfoDele(object sender, RoomInfoEventArgs e);

    public class CreateRoomEventArgs : EventArgs
    {
        public enum ErrorCode
        {
            Ok,
            UnknownCapacity,
            RoomNameOverlap,
        }

        public ErrorCode ErrCode { get; private set; }

        public Room NewRoom { get; private set; }

        public CreateRoomEventArgs(ErrorCode errcode, Room newroom)
        {
            ErrCode = errcode;
            NewRoom = newroom;
        }
    }
    public delegate void OnCreateRoomDele(object sender, CreateRoomEventArgs e);

    public class DeleteRoomEventArgs : EventArgs
    {
        public enum ErrorCode
        {
            Ok,
			UnknownID,
			RoomNotExist,
            AccessDenied,
        }

        public ErrorCode ErrCode { get; private set; }

        public uint? ID { get; private set; }

        public DeleteRoomEventArgs(ErrorCode errcode, uint? id)
        {
            ErrCode = errcode;
            ID = id;
        }
    }
    public delegate void OnDeleteRoomDele(object sender, DeleteRoomEventArgs e);

    public class EnterToRoomEventArgs : EventArgs
    {
        public enum ErrorCode
        {
            Ok,
            ClientIsFull,
            PasswordIsWrong,
            UnknownRoomID,
        }
        public ErrorCode ErrCode { get; private set; }

        public short RoomID { get; private set; }

        public string RoomName { get; private set; }

        public EnterToRoomEventArgs(ErrorCode errcode, short room_id, string roomname)
        {
            ErrCode = errcode;
            RoomID = room_id;
            RoomName = roomname;
        }
    }
    public delegate void OnEnterToRoomDele(object sender, EnterToRoomEventArgs e);

    public class ReceiveChatEventArgs : EventArgs
    {
        public short RoomID { get; private set; }

        public string Sender { get; private set; }

        public string Message { get; private set; }

        public bool IsRawMessage { get; private set; }

        public ReceiveChatEventArgs(short room_id, string sender, string message, bool raw)
        {
            RoomID = room_id;
            Sender = sender;
            Message = message;
            IsRawMessage = raw;
        }
    }
    public delegate void OnReceiveChatDele(object sender, ReceiveChatEventArgs e);

    public partial class PengChat3ClientSock
    {
        public event OnLoginDele OnLogin;
        public event OnDisconnectedDele OnDisconnected;
        public event OnRoomInfoDele OnRoomInfo;
        public event OnCreateRoomDele OnCreateRoom;
        public event OnDeleteRoomDele OnDeleteRoom;
        public event OnEnterToRoomDele OnEnterToRoom;
        public event OnReceiveChatDele OnReceiveChat;
    }
}