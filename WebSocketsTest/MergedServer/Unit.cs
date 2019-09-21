using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
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
                if (!Users.Any(x => x.Id.Equals(result)))
                {
                    Users.Add(new MessageProvider
                    {
                        Id = Count,
                        Name = result,
                        Client = new TcpClient()
                    });

                    binFormatter.Serialize(mStream, true);

                    return mStream.ToArray();
                }
                else
                {
                    binFormatter.Serialize(mStream, false);

                    return mStream.ToArray();
                }
            }
            return default;
        }
    }
}
