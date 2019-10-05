using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

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

                Console.WriteLine($"{user.Name} connected!!");

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

            //client.Client.Shutdown(SocketShutdown.Both);
            //client.Close();
        }

        public static void Broadcast(string data)
        {
            var isCommand = InterpretateCommand(data);
            if (isCommand) return;

            byte[] buffer = Encoding.ASCII.GetBytes(data + Environment.NewLine);

            lock (_lock)
            {
                foreach (var user in Users)
                {
                    if (user.IsActive)
                    {
                        NetworkStream stream = user.Client.GetStream();
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    else
                    {
                        user.LostMessages.Add(new LostMessage
                        {
                            Message = data,
                            ReceivedDate = DateTime.Now
                        });
                    }
                }
            }
        }

        public static bool InterpretateCommand(string command)
        {
            if (command.StartsWith("--set "))
            {
                var result = command.Replace("--set ", string.Empty);
                var list = result.Split(':');
                if (list.First().Equals("ClientNotActive"))
                {
                    Users.FirstOrDefault(x => x.Name.Equals(list.Last())).IsActive = false;
                    return true;
                }

            }

            return false;
        }
    }
}
