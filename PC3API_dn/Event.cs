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
            AlreadyLogged,
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
    public delegate void OnLoginHandler(object sender, LoginEventArgs e);

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
    public delegate void OnDisconnectedHandler(object sender, DisconnectedEventArgs e);

    public class RoomInfoEventArgs : EventArgs
    {
        public Room[] Rooms { get; private set; }

        public RoomInfoEventArgs(Room[] rooms)
        {
            Rooms = rooms;
        }
    }
    public delegate void OnRoomInfoHandler(object sender, RoomInfoEventArgs e);

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
    public delegate void OnCreateRoomHandler(object sender, CreateRoomEventArgs e);

    public class RemoveRoomEventArgs : EventArgs
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

        public RemoveRoomEventArgs(ErrorCode errcode, uint? id)
        {
            ErrCode = errcode;
            ID = id;
        }
    }
    public delegate void OnRemoveRoomHandler(object sender, RemoveRoomEventArgs e);

    public class AddClientEventArgs : EventArgs
    {
        public enum ErrorCode : byte
        {
            Ok,
            UnknownID,
            RoomNotExist,
            RoomIsFull,
            PasswordIsWrong,
            AlreadyEntered,
        }

        public ErrorCode ErrCode { get; private set; }

        public uint? RoomID { get; private set; }

        public Member AddedMember { get; private set; }

        public AddClientEventArgs(ErrorCode errcode, uint? room_id, Member member)
        {
            ErrCode = errcode;
            RoomID = room_id;
            AddedMember = member;
        }
    }
    public delegate void OnAddClientHandler(object sender, AddClientEventArgs e);

    public class RemoveClientEventArgs : EventArgs
    {
        public enum ErrorCode : byte
        {
            Ok,
            UnknownID,
            RoomNotExist,
        }

        public ErrorCode ErrCode { get; private set; }

        public uint? RoomID { get; private set; }

        public string RemovedMemberNickname { get; private set; }

        public RemoveClientEventArgs(ErrorCode errcode, uint? room_id, string member)
        {
            ErrCode = errcode;
            RoomID = room_id;
            RemovedMemberNickname = member;
        }
    }
    public delegate void OnRemoveClientHandler(object sender, RemoveClientEventArgs e);

    public class GetMembersEventArgs : EventArgs
    {
        public enum ErrorCode : byte
        {
            Ok,
            UnknownID,
            RoomNotExist,
        }

        public ErrorCode ErrCode { get; private set; }

        public uint? RoomID { get; private set; }

        public Member[] Members { get; private set; }

        public GetMembersEventArgs(ErrorCode errcode, uint? room_id, Member[] mem)
        {
            ErrCode = errcode;
            RoomID = room_id;
            Members = mem;
        }
    }
    public delegate void OnGetMembersHandler(object sender, GetMembersEventArgs e);

    public class ChangeStateEventArgs : EventArgs
    {
        public enum ErrorCode : byte
        {
            Ok,
            UnknownID,
            RoomNotExist,
        }

        public ErrorCode ErrCode { get; private set; }

        public uint? RoomID { get; private set; }

        public string Nickname { get; private set; }

        public Member.MemberState? State { get; private set; }

        public ChangeStateEventArgs(ErrorCode errcode, uint? room_id, string nick, Member.MemberState? state)
        {
            ErrCode = errcode;
            RoomID = room_id;
            Nickname = nick;
            State = state;
        }
    }
    public delegate void OnChangeStateHandler(object sender, ChangeStateEventArgs e);

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
    public delegate void OnReceiveChatHandler(object sender, ReceiveChatEventArgs e);

    public partial class PengChat3ClientSock
    {
        public event OnLoginHandler OnLogin;
        public event OnDisconnectedHandler OnDisconnected;
        public event OnRoomInfoHandler OnRoomInfo;
        public event OnCreateRoomHandler OnCreateRoom;
        public event OnRemoveRoomHandler OnRemoveRoom;
        public event OnAddClientHandler OnAddClient;
        public event OnRemoveClientHandler OnRemoveClient;
        public event OnGetMembersHandler OnGetMembers;
        public event OnChangeStateHandler OnChangeState;
        public event OnReceiveChatHandler OnReceiveChat;
    }
}