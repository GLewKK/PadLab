namespace MessageChannel.Abstractions
{
    public interface IMessageBroker
    {
        byte[] SerializeObject<T>(T obj);
        T DeserializeObject<T>(byte[] byteArr) where T : class;
    }
}
