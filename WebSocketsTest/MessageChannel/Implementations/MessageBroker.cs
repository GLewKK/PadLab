using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MessageChannel.Abstractions;

namespace MessageChannel.Implementations
{
    public class MessageBroker : IMessageBroker
    {
        public byte[] SerializeObject<T>(T obj)
        {
            var binFormatter = new BinaryFormatter();
            var mStream = new MemoryStream();

            binFormatter.Serialize(mStream, obj);

            return mStream.ToArray();
        }

        public T DeserializeObject<T>(byte[] byteArr) where T : class
        {
            var stream = new MemoryStream();
            var binaryFormatter = new BinaryFormatter();

            stream.Write(byteArr, 0, byteArr.Length);
            stream.Position = 0;

            return binaryFormatter.Deserialize(stream) as T;
        }
    }
}
