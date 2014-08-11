using System;

namespace PC3API_dn
{
    public class LoginEventArgs : EventArgs
    {
        public bool Successed { get; private set; }
        public string Nickname { get; private set; }

        public enum ErrorCode : byte
        {
            Ok,
            UnknownIdPw,
        }

        public ErrorCode ErrCode { get; private set; }

        public LoginEventArgs(bool successed, string nickname, ErrorCode errcode)
        {
            Successed = successed;
            Nickname = nickname;
            ErrCode = errcode;
        }
    }
    public delegate void OnLoginDele(object sender, LoginEventArgs e);

    public class ClosedEventArgs : EventArgs
    {
        public string ClosedIP { get; private set; }
        public int ClosedPort { get; private set; }

        public ClosedEventArgs(string ip, int port)
        {
            ClosedIP = ip;
            ClosedPort = port;
        }
    }
    public delegate void OnClosedDele(object sender, ClosedEventArgs e);

    public class RoomInfoEventArgs : EventArgs
    {
        public RoomInfoEventArgs()
        {

        }
    }
    public delegate void OnRoomInfoDele(object sender, RoomInfoEventArgs e);

    public class CreateRoomEventArgs : EventArgs
    {
        public enum ErrorCode : byte
        {
            Ok,
            CapacityIsNegative,
        }

        public ErrorCode ErrCode { get; private set; }

        public CreateRoomEventArgs(ErrorCode errcode)
        {
            ErrCode = errcode;
        }
    }
    public delegate void OnCreateRoomDele(object sender, CreateRoomEventArgs e);

    public class DeleteRoomEventArgs : EventArgs
    {
        public enum ErrorCode : byte
        {
            Ok,
			PermissionError,
			UnknownRoomID,
        }

        public ErrorCode ErrCode { get; private set; }

        public DeleteRoomEventArgs(ErrorCode errcode)
        {
            ErrCode = errcode;
        }
    }
    public delegate void OnDeleteRoomDele(object sender, DeleteRoomEventArgs e);

    public class EnterToRoomEventArgs : EventArgs
    {
        public enum ErrorCode : byte
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
        public event OnClosedDele OnClosed;
        public event OnRoomInfoDele OnRoomInfo;
        public event OnCreateRoomDele OnCreateRoom;
        public event OnDeleteRoomDele OnDeleteRoom;
        public event OnEnterToRoomDele OnEnterToRoom;
        public event OnReceiveChatDele OnReceiveChat;
    }
}