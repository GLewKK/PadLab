using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace TCPClient
{
    public class Program
    {
        public static string name = string.Empty;
        static ConsoleEventDelegate handler;
        static TcpClient client;
        static Thread thread;
        static NetworkStream ns;

        static void Main(string[] args)
        {
            var udpClient = new UdpClient();

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 5000;
            client = new TcpClient();

            IPEndPoint epUDP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100);


            //connect to udpServer
            udpClient.Connect(epUDP);

            var receivedData = new byte[1024 * 4];

            while (true)
            {
                Console.WriteLine("Select a username:");
                name = Console.ReadLine();

                var binFormatter = new BinaryFormatter();
                var mStream = new MemoryStream();

                binFormatter.Serialize(mStream, name);
                var sending = mStream.ToArray();

                udpClient.Send(sending, sending.Length);

                receivedData = udpClient.Receive(ref epUDP);

                var stream1 = new MemoryStream();
                var binaryFormatter1 = new BinaryFormatter();

                stream1.Write(receivedData, 0, receivedData.Length);
                stream1.Position = 0;

                var result1 = binaryFormatter1.Deserialize(stream1) as dynamic;
                if (result1 is bool)
                {
                    if (result1)
                    {
                        Console.WriteLine("Successfully added.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error! Name already exists.");
                        continue;
                    }
                }
            }
            




            client.Connect(ip, port);

            Console.WriteLine($"{name} connected!!");
            ns = client.GetStream();
            thread = new Thread(o => ReceiveData((TcpClient)o));

            thread.Start(client);

            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);

            string s;
            while (!string.IsNullOrEmpty((s = Console.ReadLine())))
            {
                byte[] buffer = Encoding.ASCII.GetBytes($"{name}: {s}");
                ns.Write(buffer, 0, buffer.Length);
            }
        }
        static void ReceiveData(TcpClient client)
        {
            NetworkStream ns = client.GetStream();
            
            byte[] receivedBytes = new byte[1024];
            int byte_count;

            while ((byte_count = ns.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
            {
                var message = Encoding.ASCII.GetString(receivedBytes, 0, byte_count);
                if (!message.Contains(name))
                {
                    Console.WriteLine(message);
                }
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                client.Client.Shutdown(SocketShutdown.Send);
                thread.Join();
                ns.Close();
                client.Close();
            }
            return false;
        }


        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
