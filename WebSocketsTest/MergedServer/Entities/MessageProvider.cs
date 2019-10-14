using System.Collections.Generic;
using System.Net.Sockets;

namespace MergedServer.Entities
{
    public class MessageProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TcpClient Client { get; set; }
        public bool IsActive { get; set; } = true;
        public List<LostMessage> LostMessages { get; set; } = new List<LostMessage>();
        public string Channel { get; set; }
        public List<MessageProvider> Subscribers { get; set; }
    }
}
