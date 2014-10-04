using System;

namespace PC3API_dn
{
    internal static class Protocol
    {
        internal const char FLAG_SUCCESSED = '1';
        internal const char FLAG_FAILED = '1';

        internal const string PROTOCOL_CHECK_REAL = "CHCK";
        internal const string PROTOCOL_LOGIN = "LGIN";
        internal const string PROTOCOL_GET_ROOM_INFO = "GTRI";
        internal const string PROTOCOL_CREATE_ROOM = "CTRM";
        internal const string PROTOCOL_REMOVE_ROOM = "DTRM";
        internal const string PROTOCOL_ADD_CLIENT = "ADCT";
        internal const string PROTOCOL_REMOVE_CLIENT = "RVCT";
        internal const string PROTOCOL_GET_MEMBERS = "GTMB";
        internal const string PROTOCOL_CHANGE_STATE = "CHST";
        internal const string PROTOCOL_MASTER_CHANGE = "MSCG";
        internal const string PROTOCOL_SEND_CHAT = "SDCT";
    }
}
