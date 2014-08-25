using System;

namespace PC3API_dn
{
    internal static class Protocol
    {
        internal const string PROTOCOL_CHECK_REAL = "CHCK";
        internal const string PROTOCOL_LOGIN = "LGIN";
        internal const string PROTOCOL_GET_ROOM_INFO = "GTRI";
        internal const string PROTOCOL_CREATE_ROOM = "CTRM";
        internal const string PROTOCOL_ADD_ROOM = "ADRM";
        internal const string PROTOCOL_DELETE_ROOM = "DTRM";
        internal const string PROTOCOL_SUB_ROOM = "SBRM";
        internal const string PROTOCOL_ADD_CLIENT = "ADCT";
        internal const string PROTOCOL_REMOVE_CLIENT = "RVCT";
        internal const string PROTOCOL_ENTRY_ROOM = "ETRM";
        internal const string PROTOCOL_EXIT_ROOM = "EXRM";
        internal const string PROTOCOL_GET_MEMBERS = "GTMB";
        internal const string PROTOCOL_CHANGE_STATE = "CHST";
    }
}
