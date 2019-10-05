using System.Net;
using System.Net.Sockets;
using MessageChannel.Abstractions;

namespace MessageChannel.Implementations
{
    public class AppUserConfigurator : IAppUserConfigurator
    {
        private readonly UdpClient _client;
        private readonly IMessageBroker _messageBroker;
        private IPEndPoint epUDP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100);

        public AppUserConfigurator()
        {
            _client = new UdpClient();
            _messageBroker = new MessageBroker();
        }

        public string RegisterUser(string username)
        {
            var arr = _messageBroker.SerializeObject<string>(username);
            _client.Send(arr, arr.Length);

            var result = _messageBroker.DeserializeObject<string>(_client.Receive(ref epUDP));

            return result;
        }

        public void ConnectToServer()
        {
            _client.Connect(epUDP);
        }
    }
}
