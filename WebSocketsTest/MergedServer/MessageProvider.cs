using System.Net.Sockets;

namespace MergedServer
{
    public class MessageProvider
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public TcpClient Client { get; set; }
    }
}
