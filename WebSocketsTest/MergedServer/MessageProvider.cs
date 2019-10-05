using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace MergedServer
{
    public class MessageProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TcpClient Client { get; set; }
        public bool IsActive { get; set; } = true;
        public List<LostMessage> LostMessages { get; set; } = new List<LostMessage>();
    }

    public class LostMessage
    {
        public string Message { get; set; }
        public DateTime ReceivedDate { get; set; }
    }
}
