using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using MergedServer.Entities;
using MergedServer.Extensions;

namespace MergedServer
{
    public class Program
    {
        public static List<MessageProvider> Users = new List<MessageProvider>();

        static readonly object _lock = new object();
        public static int Count = 1;


        static void Main(string[] args)
        {
            Thread udpServer = new Thread(UdpServer);
            Thread tcpServer = new Thread(TcpServer);

            udpServer.Start();
            tcpServer.Start();
        }

        public static void UdpServer()
        {
            while (true)
            {
                int recv;
                byte[] data = new byte[1024 * 4];

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 100);
                Socket newSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                newSocket.Bind(endPoint);
                Console.WriteLine("Waiting for client...");
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 100);
                EndPoint tmpRemote = (EndPoint)sender;

                recv = newSocket.ReceiveFrom(data, ref tmpRemote);
                Console.WriteLine($"Message received from {tmpRemote}");


                var mStream = new MemoryStream();
                var binFormatter = new BinaryFormatter();

                mStream.Write(data, 0, data.Length);
                mStream.Position = 0;

                var result = binFormatter.Deserialize(mStream) as dynamic;

                byte[] byteArr = Unit.Execute(result);


                newSocket.Connect(tmpRemote);
                if (newSocket.Connected)
                {
                    newSocket.Send(byteArr);

                }
                newSocket.Close();
            }
        }
        public static void TcpServer()
        {
            TcpListener ServerSocket = new TcpListener(IPAddress.Any, 5000);
            ServerSocket.Start();

            while (true)
            {
                TcpClient client = ServerSocket.AcceptTcpClient();
                var user = Users.FirstOrDefault(x => x.Id == Count);
                lock (_lock) user.Client = client;

                Thread t = new Thread(Handle_clients);
                t.Start(Count);
                Count++;
            }
        }


        public static void Handle_clients(object o)
        {
            int id = (int)o;
            TcpClient client;

            lock (_lock) client = Users.FirstOrDefault(x => x.Id == id).Client;
            while (true)
            {

                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int byte_count = stream.Read(buffer, 0, buffer.Length);

                if (byte_count == 0)
                {
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, byte_count);
                Broadcast(data);
                //Console.WriteLine(data);
            }
        }

        public static void Broadcast(string data)
        {
            var index = data.IndexOf(':');

            var userName = data.Substring(0, index);
            var userSource = Users.FirstOrDefault(x => x.Name.Equals(userName));

            var command = data.Substring(index, data.Length - index).TrimStart(':', ' ');
            var messageTargets = new List<MessageTarget>();

            var isNotificationCommand = false;
            if (command.Contains("--"))
            {
                isNotificationCommand = true;

                var spaceIndex = command.IndexOf(' ', 0);
                var commandName = string.Empty;
                var commandValue = string.Empty;
                if (spaceIndex >= 0)
                {
                    commandName = command.Substring(0, spaceIndex).Replace("--", string.Empty);

                    commandValue = command.Substring(spaceIndex, command.Length - spaceIndex).TrimStart(' ');
                }
                else
                {
                    commandName = command.Replace("--", string.Empty);
                }

                switch (commandName.ToLower())
                {
                    case UserCommand.CreateChannel:
                        messageTargets = userSource.CreateChannel(commandValue);
                        break;
                    case UserCommand.Publish:
                        messageTargets = userSource.Publish(commandValue);
                        isNotificationCommand = false;
                        break;
                    case UserCommand.PublishShortCommand:
                        messageTargets = userSource.Publish(commandValue);
                        isNotificationCommand = false;
                        break;
                    case UserCommand.Disconnect:
                        messageTargets = userSource.Disconnect();
                        break;
                    case UserCommand.Subscribe:
                        messageTargets = userSource.Subscribe(commandValue);
                        break;
                    case UserCommand.SubscribeShortCommand:
                        messageTargets = userSource.Subscribe(commandValue);
                        break;
                    case UserCommand.NotifyConnect:
                        messageTargets = userSource.NotifyConnect();
                        break;
                    case UserCommand.NotifyReconnect:
                        messageTargets = userSource.NotifyReconnect();
                        break;
                    case UserCommand.UnSubscribe:
                        messageTargets = userSource.Unsubscribe(commandValue);
                        break;
                    case UserCommand.UnSubscribeShortCommand:
                        messageTargets = userSource.Unsubscribe(commandValue);
                        break;
                    default:
                        messageTargets = userSource.NotFound();
                        break;
                }
            }
            else
            {
                messageTargets = userSource.SendMessage(data);
            }

            foreach (var target in messageTargets)
            {
                byte[] buffer = Encoding.ASCII.GetBytes(target.Message + Environment.NewLine);

                foreach (var user in target.Targets)
                {
                    if (user.IsActive || isNotificationCommand)
                    {
                        NetworkStream stream = user.Client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        user.AddLostMessage(target.Message);
                    }
                }
            }
        }
    }
}
