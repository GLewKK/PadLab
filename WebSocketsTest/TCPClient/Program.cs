using System;
using System.Runtime.InteropServices;
using MessageChannel.Abstractions;
using MessageChannel.Implementations;

namespace TCPClient
{
    public class Program
    {
        public static string name = string.Empty;
        static ConsoleEventDelegate handler;

        private static readonly IClientManager iManager = new ClientManager();

        static void Main(string[] args)
        {
            IAppUserConfigurator userConfigurator = new AppUserConfigurator();

            userConfigurator.ConnectToServer();

            while (true)
            {
                Console.WriteLine("Select a username:");
                name = Console.ReadLine();

                var result = userConfigurator.RegisterUser(name);

                if (result == bool.TrueString)
                {
                    Console.WriteLine("Connected.");
                    break;
                }
                if (result == bool.FalseString)
                {
                    Console.WriteLine("This user is already in chat!");
                }
                else
                {
                    Console.WriteLine(result);
                    break;
                }
            }

            iManager.SetName(name);
            iManager.ConnectToServer();
            iManager.SetNetworkStream();
            iManager.SetThread();

            handler = ConsoleEventCallback;
            SetConsoleCtrlHandler(handler, true);

            string s;
            string exitCode = "--exit";
            while (exitCode != (s = Console.ReadLine()))
            {
                if (!string.IsNullOrEmpty(s))
                    iManager.SendMessage($"{name}: {s}");
            }
        }

        static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                iManager.DisconnectFromServer();
            }
            return false;
        }


        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }
}
