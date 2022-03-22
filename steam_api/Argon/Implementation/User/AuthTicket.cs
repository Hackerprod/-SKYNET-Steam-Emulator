using System;


namespace InterfaceUser
{
    public class AuthTicket
    {
        public bool is_server_ticket = false;
        public int app_id;
        public int pipe_id;
        public uint crc32;
        public int handle;
        public byte[] ticket;
        public bool cancelled = false;
    }
}