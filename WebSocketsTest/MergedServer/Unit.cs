using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using MergedServer.Entities;
using static MergedServer.Program;

namespace MergedServer
{
    public class Unit
    {
        public static byte[] Execute(bool result)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();

            binFormatter.Serialize(mStream, Users);

            var byteArr = mStream.ToArray();

            if (!result)
            {
                return byteArr;
            }
            return default;
        }

        public static byte[] Execute(string result) 
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();

            if (!string.IsNullOrEmpty(result))
            {
                if (!Users.Any(x => x.Name.Equals(result)))
                {
                    Users.Add(new MessageProvider
                    {
                        Id = Count,
                        Name = result,
                        Client = new TcpClient(),
                        Subscribers = new List<MessageProvider>(),
                        LostMessages = new List<LostMessage>(),
                        IsActive = true,
                        Channel = string.Empty
                    });

                    binFormatter.Serialize(mStream, true.ToString());

                    Program.Broadcast($"{result}: --{UserCommand.NotifyConnect}");
                    //return mStream.ToArray();
                }

                var user = Users.FirstOrDefault(x => x.IsActive == false && x.Name.Equals(result));
                if (user != null)
                {
                    user.IsActive = true;
                    user.Client = new TcpClient();
                    user.Id = Count;

                    binFormatter.Serialize(mStream, string.Join(Environment.NewLine,user.LostMessages.Select(x => $"{x.ReceivedDate:dd/MM/yyyy HH:mm:ss} - {x.Message} ").ToList()));
                    user.LostMessages = new List<LostMessage>();

                    Program.Broadcast($"{result}: --{UserCommand.NotifyReconnect}");
                }
                else
                {
                    binFormatter.Serialize(mStream, false.ToString());
                }


                return mStream.ToArray();
            }
            return default;
        }
    }
}
