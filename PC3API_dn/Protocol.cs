using System;

enum ProtocolHeader : short
{
    // First packet, when connected to server, client need to send this packet with a few bytes
    packet_check_real,

    // Login packet, when checked client is real, client need to send this
    packet_login,
}