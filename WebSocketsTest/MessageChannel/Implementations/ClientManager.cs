using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using MessageChannel.Abstractions;

namespace MessageChannel.Implementations
{
    public class ClientManager : IClientManager
    {
        private readonly TcpClient _client;
        private Thread _thread;
        private NetworkStream _networkStream;
        private string _username;

        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 5000;

        public ClientManager()
        {
            _client = new TcpClient();
        }

        public void SetName(string name)
        {
            _username = name;
        }

        public void SendMessage(string text)
        {
            var byteArr = Encoding.ASCII.GetBytes(text);
            _networkStream.Write(byteArr, 0, byteArr.Length);
        }

        public void ConnectToServer()
        {
            _client.Connect(ip, port);
        }

        public void SetNetworkStream()
        {
            _networkStream = _client.GetStream();
        }

        public void SetThread()
        {
            _thread = new Thread(o => ReceiveData((TcpClient)o));
            _thread.Start(_client);
        }

        void ReceiveData(TcpClient client)
        {
            NetworkStream ns1 = client.GetStream();

            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns1.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                var message = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);
                Console.WriteLine(message);
            }
        }

        public void DisconnectFromServer()
        {
            SendMessage($"{_username}:--DisconnectUser");

            _client.Client.Shutdown(SocketShutdown.Send);
            _thread.Join();
        }

    }
}
